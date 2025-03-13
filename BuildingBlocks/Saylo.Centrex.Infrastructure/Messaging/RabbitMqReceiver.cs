using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Saylo.Centrex.Application.Common.Messaging;
using Saylo.Centrex.Infrastructure.Messaging.Options;

namespace Saylo.Centrex.Infrastructure.Messaging;

public class RabbitMqReceiver : IDisposable
{
    private readonly RabbitMqOptions _options;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly Dictionary<string, Type> _routingKeyToTypeMap = new();
    private readonly Dictionary<Type, List<object>> _messageHandlers = new();

    public RabbitMqReceiver(RabbitMqOptions options)
    {
        _options = options;
        _connection = new ConnectionFactory
        {
            HostName = options.HostName,
            UserName = options.UserName,
            Password = options.Password,
            AutomaticRecoveryEnabled = true,
            DispatchConsumersAsync = true
        }.CreateConnection();

        _channel = _connection.CreateModel();
    }

    // Updated to accept Func<T, MetaData, Task> instead of Action<T, MetaData>
    public void RegisterHandler<T>(Func<T, MetaData, Task> handler) where T : class
    {
        var eventName = typeof(T).Name;
        if (_options.RoutingKeys.TryGetValue(eventName, out string routingKey))
        {
            _routingKeyToTypeMap[routingKey] = typeof(T);
            
            if (!_messageHandlers.ContainsKey(typeof(T)))
            {
                _messageHandlers[typeof(T)] = new List<object>();
            }
            _messageHandlers[typeof(T)].Add(handler);
        }
    }

    public void StartConsuming()
    {
        _channel.ExchangeDeclare(_options.ExchangeName, ExchangeType.Direct, true);

        foreach (var queueBinding in _options.QueueNames)
        {
            var eventName = queueBinding.Key;
            var queueName = queueBinding.Value;
            var routingKey = _options.RoutingKeys[eventName];

            _channel.QueueDeclare(queueName, true, false, false, null);
            _channel.QueueBind(queueName, _options.ExchangeName, routingKey, null);
        }

        _channel.BasicQos(0, 1, false);

        foreach (var queueBinding in _options.QueueNames)
        {
            var queueName = queueBinding.Value;
            var consumer = new AsyncEventingBasicConsumer(_channel);
            
            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var routingKey = ea.RoutingKey;
                    if (_routingKeyToTypeMap.TryGetValue(routingKey, out var messageType))
                    {
                        var body = Encoding.UTF8.GetString(ea.Body.Span);
                        var messageGenericType = typeof(Message<>).MakeGenericType(messageType);
                        var message = JsonSerializer.Deserialize(body, messageGenericType);

                        var data = messageGenericType.GetProperty("Data").GetValue(message);
                        var metadata = (MetaData)messageGenericType.GetProperty("MetaData").GetValue(message);

                        foreach (var handler in _messageHandlers[messageType])
                        {
                            // Await the task returned by the handler
                            await (Task)((Delegate)handler).DynamicInvoke(data, metadata);
                        }
                    }
                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    // Requeue the message when an exception occurs
                    _channel.BasicNack(ea.DeliveryTag, false, true);
                    // Optional: Log the exception here if needed
                }
            };

            _channel.BasicConsume(queueName, false, consumer);
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
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
    private readonly string _faultQueueName;
    private readonly int _maxRetryAttempts;

    public RabbitMqReceiver(RabbitMqOptions options, string faultQueueName = "fault_queue", int maxRetryAttempts = 3)
    {
        _options = options;
        _faultQueueName = faultQueueName;
        _maxRetryAttempts = maxRetryAttempts;
        
        _connection = new ConnectionFactory
        {
            HostName = options.HostName,
            UserName = options.UserName,
            Password = options.Password,
            AutomaticRecoveryEnabled = true,
            DispatchConsumersAsync = true
        }.CreateConnection();

        _channel = _connection.CreateModel();
        
        // Declare the fault queue
        _channel.QueueDeclare(_faultQueueName, true, false, false, null);
    }

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
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        
                        // Extract retry count from message headers
                        int retryCount = 0;
                        if (ea.BasicProperties.Headers != null && 
                            ea.BasicProperties.Headers.TryGetValue("x-retry-count", out var retryObj))
                        {
                            retryCount = Convert.ToInt32(retryObj);
                        }

                        var messageGenericType = typeof(Message<>).MakeGenericType(messageType);
                        var deserializedMessage = JsonSerializer.Deserialize(message, messageGenericType);

                        var data = messageGenericType.GetProperty("Data").GetValue(deserializedMessage);
                        var metadata = (MetaData)messageGenericType.GetProperty("MetaData").GetValue(deserializedMessage);

                        try
                        {
                            foreach (var handler in _messageHandlers[messageType])
                            {
                                // Await the task returned by the handler
                                await (Task)((Delegate)handler).DynamicInvoke(data, metadata);
                            }
                            // If we get here, processing was successful
                            _channel.BasicAck(ea.DeliveryTag, false);
                        }
                        catch (Exception)
                        {
                            // Check if we've reached max retries
                            if (retryCount >= _maxRetryAttempts)
                            {
                                // Move to fault queue
                                var properties = _channel.CreateBasicProperties();
                                properties.Persistent = true;
                                properties.Headers = new Dictionary<string, object>
                                {
                                    { "x-original-exchange", _options.ExchangeName },
                                    { "x-original-routing-key", routingKey },
                                    { "x-exception", "Failed after max retry attempts" },
                                    { "x-retry-count", retryCount }
                                };
                                
                                _channel.BasicPublish(
                                    exchange: "",
                                    routingKey: _faultQueueName,
                                    basicProperties: properties,
                                    body: body);
                                
                                // Acknowledge the original message since we've moved it to the fault queue
                                _channel.BasicAck(ea.DeliveryTag, false);
                            }
                            else
                            {
                                // Retry the message with incremented retry count
                                var properties = _channel.CreateBasicProperties();
                                properties.Persistent = true;
                                properties.Headers = new Dictionary<string, object>
                                {
                                    { "x-retry-count", retryCount + 1 }
                                };
                                
                                // Publish back to the same queue with a delay
                                // Note: In a production environment, you might want to use a delay exchange plugin
                                // or implement a more sophisticated retry mechanism with exponential backoff
                                _channel.BasicPublish(
                                    exchange: _options.ExchangeName,
                                    routingKey: routingKey,
                                    basicProperties: properties,
                                    body: body);
                                
                                // Acknowledge the original message since we've republished it
                                _channel.BasicAck(ea.DeliveryTag, false);
                            }
                        }
                    }
                    else
                    {
                        // Unknown routing key, acknowledge and move on
                        _channel.BasicAck(ea.DeliveryTag, false);
                    }
                }
                catch (Exception ex)
                {
                    // Unexpected error in message processing logic
                    // Reject the message without requeue
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                    // Optional: Log the exception here
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
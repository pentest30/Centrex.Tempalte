using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Saylo.Centrex.Application.Common.Messaging;
using Saylo.Centrex.Infrastructure.Messaging.Options;

namespace Saylo.Centrex.Infrastructure.Messaging;

public class RabbitMqSender<T> : IMessageSender<T> where T : class
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly string _exchangeName;
    private readonly string _routingKey;

    public RabbitMqSender(RabbitMqSenderOptions options)
    {
        _connectionFactory = new ConnectionFactory
        {
            HostName = options.HostName,
            UserName = options.UserName,
            Password = options.Password
        };

        _exchangeName = options.ExchangeName;
        _routingKey = options.RoutingKey;
    }

    public async Task SendAsync<T>(T message, MetaData metaData = null, CancellationToken cancellationToken = default)
    {
        await Task.Run(() =>
        {
            using var connection = _connectionFactory.CreateConnection();
            using var channel = connection.CreateModel();
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new Message<T>
            {
                Data = message,
                MetaData = metaData
            }));
            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            channel.BasicPublish(exchange: _exchangeName,
                routingKey: _routingKey,
                basicProperties: properties,
                body: body);
        }, cancellationToken);
    }
}
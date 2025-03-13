using Microsoft.Extensions.DependencyInjection;
using Saylo.Centrex.Application.Common.Messaging;

namespace Saylo.Centrex.Infrastructure.Messaging;

public class RabbitMqMessageBus : IMessageBus
{
    private readonly IServiceProvider _serviceProvider;

    public RabbitMqMessageBus(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public Task SendAsync<T>(T message, MetaData metaData = null, CancellationToken cancellationToken = default) where T : class
    {
        var sender = _serviceProvider.GetRequiredService<IMessageSender<T>>();
        return sender.SendAsync(message, metaData, cancellationToken);
    }
}
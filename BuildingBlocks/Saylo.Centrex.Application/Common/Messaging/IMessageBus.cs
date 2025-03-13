namespace Saylo.Centrex.Application.Common.Messaging;

public interface IMessageBus
{
    Task SendAsync<T>(T message, MetaData metaData = null, CancellationToken cancellationToken = default) where T : class;
}

public interface IMessageConsumer<T> where T : class
{
    Task ConsumeAsync(Message<T> message, CancellationToken cancellationToken);
}
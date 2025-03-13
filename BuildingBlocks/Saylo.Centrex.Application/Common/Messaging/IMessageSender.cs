namespace Saylo.Centrex.Application.Common.Messaging;

public interface IMessageSender<T> where T : class
{
    Task SendAsync<T>(T message, MetaData metaData = null, CancellationToken cancellationToken = default);
}
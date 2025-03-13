using MediatR;

namespace Saylo.Centrex.Application.Common.Events;

public interface IEventPublisher
{
    Task PublishAsync(INotification @event, CancellationToken cancellationToken = default);
}
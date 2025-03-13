using MediatR;
using Microsoft.Extensions.Logging;
using Saylo.Centrex.Application.Common.Events;

namespace Saylo.Centrex.Infrastructure.Services;
public class EventPublisher : IEventPublisher
{
    private readonly ILogger<EventPublisher> _logger;
    private readonly IPublisher _mediator;
    public EventPublisher(ILogger<EventPublisher> logger, IPublisher mediator) =>
        (_logger, _mediator) = (logger, mediator);
    public Task PublishAsync(INotification @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Publishing Event : {event}", @event.GetType().Name);
        return _mediator.Publish(@event, cancellationToken);
    }
}
using MediatR;
using Saylo.Centrex.Application.Common.Events;
using Saylo.Centrex.Application.Common.Interfaces;
using Saylo.Centrex.Application.Multitenancy;
using Saylo.Centrex.Domain.Events;

namespace Saylo.Centrex.Application.Common.Handlers;
public abstract class DomainEventHandler<TNotification> : INotificationHandler<TNotification>
    where TNotification : IDomainEvent
{
    private readonly IOutboxService _service;
    private readonly ITenantContextAccessor _tenantContextAccessor;

    protected DomainEventHandler(
        IOutboxService service,
        ITenantContextAccessor tenantContextAccessor)
    {
        _service = service;
        _tenantContextAccessor = tenantContextAccessor;
    }
    protected abstract IIntegrationEvent MapToIntegrationEvent(TNotification domainEvent);

    async Task INotificationHandler<TNotification>.Handle(TNotification notification,
        CancellationToken cancellationToken)
    {
        var integrationEvent = MapToIntegrationEvent(notification);
        integrationEvent.TenantId = _tenantContextAccessor.TenantId;
        await _service.AddAsync(integrationEvent, cancellationToken);
        await Handle(notification);
    }
    protected abstract Task Handle(TNotification notification);
}
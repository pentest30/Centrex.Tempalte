using Saylo.Centrex.Application.Common.Events;
using Saylo.Centrex.Application.Common.Handlers;
using Saylo.Centrex.Application.Common.Interfaces;
using Saylo.Centrex.Application.Multitenancy;
using Saylo.Centrex.Identity.Core.Application.IntegrationEvents.Identity;
using Saylo.Centrex.Identity.Core.Domain.DomainEvents.Identity;

namespace Saylo.Centrex.Identity.Core.Application.EventHandlers.Identity;

public class IdentityEventHandler : DomainEventHandler<UserCreatedEvent>
{
    public IdentityEventHandler(IOutboxService service,ITenantContextAccessor tenantContextAccessor) : base(service, tenantContextAccessor)
    {
    }

    protected override IIntegrationEvent MapToIntegrationEvent(UserCreatedEvent domainEvent)
    {
        return new UserCreatedIntegrationEvent(domainEvent);
    }

    protected override async Task Handle(UserCreatedEvent notification)
    {
        // nada
    }
}
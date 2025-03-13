using Saylo.Centrex.Application.Common.Events;
using Saylo.Centrex.Identity.Core.Domain.DomainEvents.AdministrationDomains;

namespace Saylo.Centrex.Identity.Core.Application.IntegrationEvents.Tenants;

public class AdministrationDomainCreatedIntegrationEvent : IIntegrationEvent
{
    public AdministrationDomainCreatedIntegrationEvent()
    {
    }

    public AdministrationDomainCreatedIntegrationEvent(AdministrationDomainCreatedEvent @event )
    {
        Name = @event.AdministrationDomain.Name;
        IsActive = @event.AdministrationDomain.IsActive;
        Email = @event.AdministrationDomain.Email;
        Administrator = @event.AdministrationDomain.Administrator;
        Id = Guid.NewGuid();
        CorrelationId = @event.CorrelationId;
        TransactionId = @event.TransactionId;
        EntityId = @event.AdministrationDomain.Id;
    }

    public Guid EntityId { get; set; }
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; }
    public string? Email { get; set; }
    public string? Administrator { get; set; }
    public object Id { get; set; }
    public string? CorrelationId { get; set; }
    public Guid? TransactionId { get; set; }
    public Guid? TenantId { get; set; }
}
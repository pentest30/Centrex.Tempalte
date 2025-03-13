using Saylo.Centrex.Domain.Events;
using Saylo.Centrex.Identity.Core.Domain.Entities;

namespace Saylo.Centrex.Identity.Core.Domain.DomainEvents.AdministrationDomains;

public class AdministrationDomainCreatedEvent : IDomainEvent
{
    public AdministrationDomain AdministrationDomain { get; set; }
    public AdministrationDomainCreatedEvent(AdministrationDomain administrationDomain, string ? correlationId)
    {
        AdministrationDomain = administrationDomain;
        CorrelationId = correlationId;
    }

    public string? CorrelationId { get; set; }
    public Guid? TransactionId { get; set; }
}
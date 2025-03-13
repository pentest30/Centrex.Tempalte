using Saylo.Centrex.Domain.Events;
using Saylo.Centrex.Identity.Core.Domain.Entities;

namespace Saylo.Centrex.Identity.Core.Domain.DomainEvents.AdministrationDomains;

public class EnterpriseCreatedEvent : IDomainEvent
{
    public EnterpriseCreatedEvent()
    {
    }

    public EnterpriseCreatedEvent(Enterprise enterprise, string correlationId)
    {
        Enterprise = enterprise;
        CorrelationId = correlationId;
    }
    public Enterprise Enterprise { get; set; } 
    public string? CorrelationId { get; set; } 
    public Guid? TransactionId { get; set; }
}
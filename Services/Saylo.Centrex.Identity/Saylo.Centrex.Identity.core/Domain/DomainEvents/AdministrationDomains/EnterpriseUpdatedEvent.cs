using Saylo.Centrex.Domain.Events;
using Saylo.Centrex.Identity.Core.Domain.Entities;

namespace Saylo.Centrex.Identity.Core.Domain.DomainEvents.AdministrationDomains;

public record EnterpriseUpdatedEvent(Enterprise Enterprise, string? CorrelationId) : IDomainEvent
{
    public Enterprise Enterprise { get; set; } = Enterprise;

    public string? CorrelationId { get; set; } = CorrelationId;
    public Guid? TransactionId { get; set; }
}
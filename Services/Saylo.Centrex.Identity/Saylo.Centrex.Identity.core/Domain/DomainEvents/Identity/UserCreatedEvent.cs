using Saylo.Centrex.Domain.Events;
using Saylo.Centrex.Identity.Core.Domain.Entities.Identity;

namespace Saylo.Centrex.Identity.Core.Domain.DomainEvents.Identity;

public class UserCreatedEvent : IDomainEvent
{
    public UserCreatedEvent()
    {
    }
    public ApplicationUser ApplicationUser { get; set; }
    public UserCreatedEvent(ApplicationUser user, string? correlationId)
    {
        ApplicationUser = user;
        CorrelationId = correlationId;
    }

    public string? CorrelationId { get; set; }
    public Guid? TransactionId { get; set; }
}

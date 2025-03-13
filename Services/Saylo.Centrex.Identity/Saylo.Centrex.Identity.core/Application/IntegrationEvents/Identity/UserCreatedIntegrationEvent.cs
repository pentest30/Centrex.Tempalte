using Saylo.Centrex.Application.Common.Events;
using Saylo.Centrex.Identity.Core.Domain.DomainEvents.Identity;

namespace Saylo.Centrex.Identity.Core.Application.IntegrationEvents.Identity;

public class UserCreatedIntegrationEvent : IIntegrationEvent
{
    public UserCreatedIntegrationEvent()
    {
    }
    public UserCreatedIntegrationEvent(UserCreatedEvent @event)
    {
        Id = Guid.NewGuid();
        UseName = @event.ApplicationUser.UserName;
        Email = @event.ApplicationUser.Email;
        UserId = @event.ApplicationUser.Id;
        CorrelationId = @event.CorrelationId;
        TransactionId = @event.TransactionId;
        FirstName = @event.ApplicationUser.FirstName;
        LastName = @event.ApplicationUser.LastName;
    }
    public string? UseName { get; set; }
    public string? Email { get; set; }
    public Guid UserId { get; set; }
    public object Id { get; set; }
    public string? CorrelationId { get; set; }
    public Guid? TransactionId { get; set; }
    public Guid? TenantId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
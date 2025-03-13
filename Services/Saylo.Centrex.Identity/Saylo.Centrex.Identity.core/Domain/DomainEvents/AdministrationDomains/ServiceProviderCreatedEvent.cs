using Saylo.Centrex.Domain.Events;
using Saylo.Centrex.Identity.Core.Domain.Entities;

namespace Saylo.Centrex.Identity.Core.Domain.DomainEvents.AdministrationDomains;

public class ServiceProviderCreatedEvent : IDomainEvent
{
    public ServiceProviderCreatedEvent()
    {
    }
    public ServiceProviderCreatedEvent(ServiceProvider serviceProvider, string correlationId)
    {
        ServiceProvider = serviceProvider;
        CorrelationId = correlationId;
    }
    public ServiceProvider ServiceProvider { get; set; }
    public string? CorrelationId { get; set; }
    public Guid? TransactionId { get; set; }
}
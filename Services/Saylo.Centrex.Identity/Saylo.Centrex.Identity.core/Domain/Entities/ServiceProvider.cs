using Saylo.Centrex.Domain.Entities;
using Saylo.Centrex.Domain.Events;
using Saylo.Centrex.Identity.Core.Domain.DomainEvents.AdministrationDomains;

namespace Saylo.Centrex.Identity.Core.Domain.Entities
{
    public class ServiceProvider : AggregateRoot<Guid>, ITenantEntity
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public string ServiceType { get; private set; }
        public string ContactEmail { get; private set; }
        public Guid? TenantId { get; set; }
        public ServiceProvider CreateNewServiceProvider(string name, string? description, 
            string serviceType, string contactEmail, string correlationId)
        {
            var serviceProvider = new ServiceProvider
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                ServiceType = serviceType,
                ContactEmail = contactEmail
            };
            serviceProvider.AddDomainEvent(new ServiceProviderCreatedEvent(serviceProvider, correlationId));
            return serviceProvider;
        }

        public void UpdateInfo(string requestName, string? requestDescription, string requestServiceType, string requestContactEmail, string correlationId)
        {
            Name = requestName;
            Description = requestDescription;
            ServiceType = requestServiceType;
            ContactEmail = requestContactEmail;
            AddDomainEvent(new ServiceProidevUpdateEvent(this, correlationId));
        }
    }

    public class ServiceProidevUpdateEvent : IDomainEvent
    {
        public ServiceProidevUpdateEvent()
        {
        }
        public ServiceProidevUpdateEvent(ServiceProvider serviceProvider, string correlationId)
        {
            Provider = serviceProvider;
            CorrelationId = correlationId;
        }

        public ServiceProvider Provider { get; set; }

        public string? CorrelationId { get; set; }
        public Guid? TransactionId { get; set; }
    }
}

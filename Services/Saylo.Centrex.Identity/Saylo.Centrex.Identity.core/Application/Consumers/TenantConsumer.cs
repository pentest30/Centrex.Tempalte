using Saylo.Centrex.Application.Common.Messaging;
using Saylo.Centrex.Identity.Core.Application.IntegrationEvents.Tenants;

namespace Saylo.Centrex.Identity.Core.Application.Consumers;

public class TenantConsumer : IMessageConsumer<AdministrationDomainCreatedIntegrationEvent>
{
    public Task ConsumeAsync(Message<AdministrationDomainCreatedIntegrationEvent> message, CancellationToken cancellationToken)
    {
        Console.WriteLine(message.Data.Id + nameof(TenantConsumer));
        return Task.CompletedTask;
    }
}
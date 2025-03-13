using Saylo.Centrex.Application.Common.Events;
using Saylo.Centrex.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Saylo.Centrex.Infrastructure.Persistence;



public static class DbContextExtensions
{
    public static async Task DispatchDomainEventsAsync(this DbContext context, Guid transactionId,  IEventPublisher eventPublisher, CancellationToken cancellationToken = default)
    {
        bool hasEvents;
        do
        {
            var entitiesWithEvents = context.ChangeTracker
                .Entries<IAggregateRoot>()
                .Select(entry => entry.Entity)
                .Where(entity => entity.DomainEvents.Count != 0)
                .ToArray();

            hasEvents = entitiesWithEvents.Any();

            foreach (var entity in entitiesWithEvents)
            {
                var domainEvents = entity.DomainEvents.ToArray();
                entity.ClearDomainEvents();
                
                foreach (var domainEvent in domainEvents)
                {
                    domainEvent.TransactionId = transactionId;
                    await eventPublisher.PublishAsync(domainEvent, cancellationToken);
                }
            }
        } while (hasEvents);
    }
}

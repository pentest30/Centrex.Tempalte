using System.ComponentModel.DataAnnotations.Schema;
using Saylo.Centrex.Domain.Events;

namespace Saylo.Centrex.Domain.Entities;

public abstract class AggregateRoot<TKey> : Entity<TKey> , IAggregateRoot
{
    [NotMapped] private readonly List<IDomainEvent> _domainEvents = new();

    [NotMapped] public new IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saylo.Centrex.Identity.Core.Domain.Entities
{
    public interface IHasKey<T>
    {
        T Id { get; set; }
    }
    public interface IAggregateRoot
    {
    }
    public interface IDomainEvent
    {
    }
    public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
    {
        [NotMapped] private readonly List<IDomainEvent> domainEvents = new();

        [NotMapped] public new IReadOnlyCollection<IDomainEvent> DomainEvents => domainEvents.AsReadOnly();

        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            domainEvents.Clear();
        }
    }
    public abstract class Entity<TKey> : IHasKey<TKey>
    {
        public TKey Id { get; set; }

        public DateTimeOffset CreatedDateTime { get; set; }

        public DateTimeOffset? UpdatedDateTime { get; set; }
    }
}

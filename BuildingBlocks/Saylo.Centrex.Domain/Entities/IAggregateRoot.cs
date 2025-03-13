using Saylo.Centrex.Domain.Events;

namespace Saylo.Centrex.Domain.Entities
{
    public interface IAggregateRoot
    {
        public new IReadOnlyCollection<IDomainEvent> DomainEvents
        {
            get;
        }

        void ClearDomainEvents();
    }
}

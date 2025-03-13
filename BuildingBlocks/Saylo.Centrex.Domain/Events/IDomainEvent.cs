using MediatR;

namespace Saylo.Centrex.Domain.Events;

public interface IDomainEvent : INotification
{
    string? CorrelationId { get; set; }
    public Guid? TransactionId { get; set; }
}
using Saylo.Centrex.Application.Common.Events;
using Saylo.Centrex.Domain.Entities;
using Saylo.Centrex.Domain.Repositories;

namespace Saylo.Centrex.Application.Common.Interfaces;

public interface IOutboxService
{
    public IUnitOfWork UnitOfWork { get; }
    Task AddAsync(IIntegrationEvent @event, CancellationToken cancellationToken);
    Task<List<OutboxEvent>> GetAllUnpublishedMessagesAsync(CancellationToken cancellationToken);
    Task MarkAsPublishedAsync(Guid messageId, CancellationToken cancellationToken = default);
    Task MarkAsProcessingAsync(Guid messageId, CancellationToken cancellationToken);
    Task MarkAsFailedAsync(Guid messageId, string error, CancellationToken cancellationToken);
    Task<OutboxEvent> GetByIdAsync(Guid messageId, CancellationToken cancellationToken);
    Task<List<OutboxEvent>> GetEventsByProcessingIdAsync(string transactionId, CancellationToken cancellationToken);
}
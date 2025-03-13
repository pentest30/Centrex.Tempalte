using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Saylo.Centrex.Application.Common.Events;
using Saylo.Centrex.Application.Common.Interfaces;
using Saylo.Centrex.Domain.Entities;
using Saylo.Centrex.Domain.Repositories;

namespace Saylo.Centrex.Infrastructure.Services.Outbox;
public class OutboxService : IOutboxService
{
    private readonly IRepository<OutboxEvent, Guid> _outboxRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    public OutboxService(
        IRepository<OutboxEvent, Guid> outboxRepository,
        IDateTimeProvider dateTimeProvider)
    {
        _outboxRepository = outboxRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public IUnitOfWork UnitOfWork => _outboxRepository.UnitOfWork;

    public async Task AddAsync(IIntegrationEvent @event, CancellationToken cancellationToken)
    {
        var serializerSettings = GetJsonSerializerSettings();
        await _outboxRepository.AddAsync(new OutboxEvent
        {
            Id = Guid.NewGuid(),
            EventType = GetFullTypeAndAssemblyName(@event),
            ObjectId = @event.Id.ToString(),
            ProcessingId = @event.TransactionId.ToString(),
            Error = string.Empty,
            Payload = JsonConvert.SerializeObject(@event, serializerSettings),
            Status = OutboxStatus.Created
        }, cancellationToken);
    }

    private static JsonSerializerSettings GetJsonSerializerSettings()
    {
        var serializerSettings = new JsonSerializerSettings 
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        return serializerSettings;
    }

    private static string GetFullTypeAndAssemblyName(object @class)
    {
        var type = @class.GetType();
        var assemblyName = type.Assembly.GetName().Name; // Get only the assembly name without version, culture, etc.
        return $"{type.FullName}, {assemblyName}";
    }


    public Task<List<OutboxEvent>> GetAllUnpublishedMessagesAsync(CancellationToken cancellationToken)
    {
        return _outboxRepository
            .GetAll()
            .AsNoTracking()
            .Where(x=>x.Status != OutboxStatus.Published)
            .OrderBy(o=> o.CreatedDateTime)
            .ToListAsync(cancellationToken: cancellationToken);
    }
    public async Task MarkAsPublishedAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        var message = await GetUnpublishedOutboxEvent(messageId, cancellationToken);
        if (message != null)
        {
            message.Status = OutboxStatus.Published;
            message.PublishedDate = _dateTimeProvider.Now;
            message.Error = string.Empty;
            await _outboxRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task MarkAsProcessingAsync(Guid messageId, CancellationToken cancellationToken)
    {
        var message = await GetUnpublishedOutboxEvent(messageId, cancellationToken);
        if (message != null)
        {
            message.Status = OutboxStatus.Processing;
            message.Error = string.Empty;
            await _outboxRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task<OutboxEvent?> GetUnpublishedOutboxEvent(Guid messageId, CancellationToken cancellationToken)
    {
        var message = await _outboxRepository
            .GetAll()
            .FirstOrDefaultAsync(x=>x.Id == messageId && x.Status!= OutboxStatus.Published, cancellationToken);
        return message;
    }

    public async Task MarkAsFailedAsync(Guid messageId, string error, CancellationToken cancellationToken)
    {
        var message = await GetUnpublishedOutboxEvent(messageId, cancellationToken);
        if (message != null)
        {
            message.Error = error;
            message.Status = OutboxStatus.Failed;
            await _outboxRepository.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
        
    }

    public Task<OutboxEvent> GetByIdAsync(Guid messageId, CancellationToken cancellationToken)
    {
        return _outboxRepository
            .GetAll()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == messageId, cancellationToken: cancellationToken);
    }
    public Task<List<OutboxEvent>> GetEventsByProcessingIdAsync(string transactionId, CancellationToken cancellationToken)
    {
        return _outboxRepository
            .GetAll()
            .Where(x => x.ProcessingId == transactionId)
            .ToListAsync(cancellationToken: cancellationToken);
    }
}
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saylo.Centrex.Application.Common.Interfaces;
using Saylo.Centrex.Application.Common.Messaging;
using Saylo.Centrex.Domain.Entities;
using Saylo.Centrex.Domain.Repositories;

namespace Saylo.Centrex.Infrastructure.Services;

public sealed class IntegrationEventPublisherService : IIntegrationEventPublisherService
{
    private readonly IMessageBus _messageBus;
    private readonly IMessageDeserializer _messageDeserializer;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<IntegrationEventPublisherService> _logger;

    public IntegrationEventPublisherService(
        IMessageBus messageBus,
        IMessageDeserializer messageDeserializer,
        IDateTimeProvider dateTimeProvider,
        ILogger<IntegrationEventPublisherService> logger)
    {
        _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
        _messageDeserializer = messageDeserializer ?? throw new ArgumentNullException(nameof(messageDeserializer));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task PublishIntegrationEventAsync(
        IUnitOfWork unitOfWork,
        Guid transactionId,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(unitOfWork);
        
        var dbContext = unitOfWork as DbContext 
            ?? throw new ArgumentException("UnitOfWork must be of type DbContext", nameof(unitOfWork));

        _logger.LogInformation("Starting to publish integration events for transaction {TransactionId}", transactionId);

        var outboxEvents = await GetPendingEventsAsync(dbContext, transactionId, cancellationToken);
        
        foreach (var outboxEvent in outboxEvents)
        {
            await ProcessOutboxEventAsync(dbContext, outboxEvent, cancellationToken);
        }
    }

    private async Task ProcessOutboxEventAsync(
        DbContext dbContext, 
        OutboxEvent outboxEvent,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogDebug("Processing outbox event {EventId} of type {EventType}",
                outboxEvent.Id, outboxEvent.EventType);

            var payload = DeserializeEventPayload(outboxEvent);
            if (payload == null) return;

            var metadata = CreateEventMetadata(outboxEvent, payload.TenantId);

            await PublishEventAsync(dbContext, outboxEvent, payload, metadata, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish event {EventId}", outboxEvent.Id);
        }
    }

    private dynamic? DeserializeEventPayload(OutboxEvent outboxEvent)
    {
        var payload = _messageDeserializer.Deserialize(outboxEvent.Payload, outboxEvent.EventType);
        if (payload == null)
        {
            _logger.LogWarning("Failed to deserialize event {EventId}", outboxEvent.Id);
        }
        return payload;
    }

    private MetaData CreateEventMetadata(OutboxEvent outboxEvent, Guid? tenantId)
    {
        return new MetaData(
            outboxEvent.ProcessingId,
            outboxEvent.ObjectId,
            tenantId,
            outboxEvent.CreatedDateTime);
    }

    private async Task PublishEventAsync(
        DbContext dbContext,
        OutboxEvent outboxEvent,
        dynamic payload,
        MetaData metadata,
        CancellationToken cancellationToken)
    {
        await _messageBus.SendAsync(payload, metadata, cancellationToken);
        await UpdateOutboxEventStatusAsync(dbContext, outboxEvent.Id);
        _logger.LogInformation("Successfully published event {EventId}", outboxEvent.Id);
    }

    private static async Task<List<OutboxEvent>> GetPendingEventsAsync(
        DbContext dbContext,
        Guid transactionId,
        CancellationToken cancellationToken)
    {
        return await dbContext.Set<OutboxEvent>()
            .AsNoTracking()
            .Where(x => x.ProcessingId == transactionId.ToString() && x.Status == OutboxStatus.Created)
            .ToListAsync(cancellationToken);
    }

    private async Task UpdateOutboxEventStatusAsync(
        DbContext dbContext,
        Guid outboxEventId)
    {
        var schema = GetSchemaName(dbContext);
        var tableName = string.IsNullOrEmpty(schema)
            ? $"[dbo].[{nameof(OutboxEvent)}]"
            : $"[{schema}].[{nameof(OutboxEvent)}]";

        var sql = $@"
            UPDATE {tableName}
            SET PublishedDate = @PublishedDate, 
                Status = @Status
            WHERE Id = @OutboxEventId";

        await dbContext.Database.ExecuteSqlRawAsync(
            sql,
            new SqlParameter("@PublishedDate", _dateTimeProvider.Now),
            new SqlParameter("@Status", OutboxStatus.Published),
            new SqlParameter("@OutboxEventId", outboxEventId));
    }
    private static string? GetSchemaName(DbContext dbContext)
    {
        return dbContext.Model
            .GetEntityTypes()
            .FirstOrDefault(x => x.ClrType == typeof(OutboxEvent))
            ?.GetSchema();
    }
}
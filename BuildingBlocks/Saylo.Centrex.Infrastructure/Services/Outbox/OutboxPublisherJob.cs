using Hangfire;
using Microsoft.Extensions.Logging;
using Saylo.Centrex.Application.Common.Interfaces;
using Saylo.Centrex.Application.Common.Messaging;
using Saylo.Centrex.Application.Multitenancy;
using Saylo.Centrex.Domain.Entities;
namespace Saylo.Centrex.Infrastructure.Services.Outbox;

public class OutboxPublisherJob : IJob
{
    private readonly IOutboxService _outboxService;
    private readonly IMessageBus _messageBus;
    private readonly ILogger<OutboxPublisherJob> _logger;
    private readonly IMessageDeserializer _messageDeserializer;

    public OutboxPublisherJob(
        IOutboxService outboxService,
        IMessageBus messageBus,
        ILogger<OutboxPublisherJob> logger,
        IMessageDeserializer messageDeserializer)
    {
        _outboxService = outboxService;
        _messageBus = messageBus;
        _logger = logger;
        _messageDeserializer = messageDeserializer;
    }

    [MaximumConcurrentExecutions(1)]
    [AutomaticRetry(Attempts = 0)]
    [DisableConcurrentExecution(timeoutInSeconds: 60)]
    public async Task ProcessAsync(CancellationToken cancellationToken = default)
    {
        var messages = await _outboxService.GetAllUnpublishedMessagesAsync(cancellationToken);

        foreach (var message in messages)
        {
            await ProcessMessageAsync(message, cancellationToken);
        }
    }

    private async Task ProcessMessageAsync(OutboxEvent message, CancellationToken cancellationToken)
    {
        try
        {
            var payload = _messageDeserializer.Deserialize(message.Payload, message.EventType);
            if (payload == null) return;

            await PublishMessageAsync(message, payload, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to process message. EventType: {EventType}, MessageId: {MessageId}",
                message.EventType,
                message.Id);
        }
    }

    private async Task PublishMessageAsync(OutboxEvent message, dynamic payload, CancellationToken cancellationToken)
    {
        try
        {
            if (await IsMessageAlreadyPublished(message.Id, cancellationToken))
            {
                _logger.LogInformation("Message {MessageId} was already published", message.Id);
                return;
            }

            var metadata = new MetaData(
                message.Id.ToString(),
                payload.Id.ToString(),
                payload.TenantId,
                message.CreatedDateTime);

            // Attempt to publish and mark as published atomically
            await PublishEventWithRetryAsync(message, payload, metadata, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish message {MessageId}", message.Id);
            throw;
        }
    }

    private async Task<bool> IsMessageAlreadyPublished(Guid messageId, CancellationToken cancellationToken)
    {
        var outboxMessage = await _outboxService.GetByIdAsync(messageId, cancellationToken);
        return outboxMessage?.PublishedDate != null;
    }
    
    private async Task PublishEventWithRetryAsync(OutboxEvent message, dynamic payload, MetaData metadata, CancellationToken cancellationToken)
    {
        try
        {
            using (await _outboxService.UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken))
            {
                var status = await GetStatusAsync(message.Id, cancellationToken);
                if (status == OutboxStatus.Published)
                {
                    _logger.LogInformation("Message {MessageId} already published", message.Id);
                    return;
                }
                if (status != OutboxStatus.Processing)
                {
                    await _outboxService.MarkAsProcessingAsync(message.Id, cancellationToken);
                    await _outboxService.UnitOfWork.CommitTransactionAsync(cancellationToken);
                }
            }

            await _messageBus.SendAsync(payload, metadata, cancellationToken);
            
            using (await _outboxService.UnitOfWork.BeginTransactionAsync(cancellationToken: cancellationToken))
            {
                await _outboxService.MarkAsPublishedAsync(message.Id, cancellationToken);
                await _outboxService.UnitOfWork.CommitTransactionAsync(cancellationToken);
                _logger.LogInformation("Message published and marked as published. MessageId: {MessageId}", message.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process message {MessageId} after all retry attempts", message.Id);
            await _outboxService.MarkAsFailedAsync(message.Id, ex.Message, cancellationToken);
            throw;
        }
    }
    private async Task<OutboxStatus> GetStatusAsync(Guid messageId, CancellationToken cancellationToken)
    {
        var @event = await _outboxService.GetByIdAsync(messageId, cancellationToken);
        return @event.Status;
    }
    
}
using Saylo.Centrex.Application.Common.Messaging.Filters;
using Saylo.Centrex.Application.Multitenancy;
using Microsoft.Extensions.Logging;

namespace Saylo.Centrex.Infrastructure.Messaging.Filters;

public class TenantMessageFilter<T> : IMessageFilter<T> where T : class
{
    private readonly ITenantContextAccessor _tenantContextAccessor;
    private readonly ILogger<TenantMessageFilter<T>> _logger;

    public TenantMessageFilter(
        ITenantContextAccessor tenantContextAccessor,
        ILogger<TenantMessageFilter<T>> logger)
    {
        _tenantContextAccessor = tenantContextAccessor ?? throw new ArgumentNullException(nameof(tenantContextAccessor));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<bool> Handle(MessageFilterContext<T> context, Func<Task<bool>> next)
    {
        
        try
        {
            if (context.Metadata.TenantId == null)
            {
                _logger.LogWarning("Message received without tenant ID. Message type: {MessageType}", typeof(T).Name);
                return false; // Don't process messages without tenant ID
            }

            _logger.LogInformation("Setting tenant context to {TenantId} for message type {MessageType}", context.Metadata.TenantId, typeof(T).Name);
                
            _tenantContextAccessor.TenantId = context.Metadata.TenantId; 
            return await next();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while processing message for tenant {TenantId}", 
                context.Metadata?.TenantId);
            throw;
        }
    }
}


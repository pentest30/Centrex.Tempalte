using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Saylo.Centrex.Application.Common.Messaging.Filters;

namespace Saylo.Centrex.Infrastructure.Messaging.ConsumerConfiguration;

public class MessageFilterChainHandler : IMessageFilterChainHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MessageFilterChainHandler> _logger;

    public MessageFilterChainHandler(
        IServiceProvider serviceProvider,
        ILogger<MessageFilterChainHandler> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task ExecuteFilterChainAsync(MessageFilterContext<object> context, CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var filters = scope.ServiceProvider.GetServices<IMessageFilter<object>>().ToList();

        if (!filters.Any())
        {
            return;
        }

        int currentFilterIndex = 0;

        async Task<bool> ExecuteNextFilter()
        {
            if (currentFilterIndex >= filters.Count)
            {
                return true;
            }

            var currentFilter = filters[currentFilterIndex];
            currentFilterIndex++;

            try
            {
                return await currentFilter.Handle(context, ExecuteNextFilter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing filter {FilterType}", currentFilter.GetType().Name);
                throw;
            }
        }

        await ExecuteNextFilter();
    }
}
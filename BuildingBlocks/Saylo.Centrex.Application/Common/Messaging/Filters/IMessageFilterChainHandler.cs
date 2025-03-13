namespace Saylo.Centrex.Application.Common.Messaging.Filters;

public interface IMessageFilterChainHandler
{
    Task ExecuteFilterChainAsync(MessageFilterContext<object> context, CancellationToken cancellationToken);
}
using MediatR;
using Microsoft.Extensions.Logging;

namespace Saylo.Centrex.Infrastructure.Behaviors.Logging;

public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

    public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        try
        {
            _logger.LogInformation("Handling {RequestName} {@Request}", requestName, request);
            var response = await next();
            _logger.LogInformation("Handled {RequestName} {@Request}", requestName, request);
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Uncaught exception during processing {RequestName} {@Request}", requestName, request);
            throw;
        }
    }
}





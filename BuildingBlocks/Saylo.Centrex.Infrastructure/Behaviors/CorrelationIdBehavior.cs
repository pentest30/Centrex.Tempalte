namespace Saylo.Centrex.Infrastructure.Behaviors;

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

public class CorrelationIdBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<CorrelationIdBehavior<TRequest, TResponse>> _logger;

    public CorrelationIdBehavior(ILogger<CorrelationIdBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var correlationId = GetCorrelationId(request);
        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
            SetCorrelationId(request, correlationId);
        }

        _logger.LogInformation("Processing {RequestName} with CorrelationId: {CorrelationId}",
            typeof(TRequest).Name, correlationId);
        var response = await next();

        _logger.LogInformation("Finished processing {RequestName} with CorrelationId: {CorrelationId}",
            typeof(TRequest).Name, correlationId);

        return response;
    }

    private string? GetCorrelationId(TRequest request)
    {
        var property = request.GetType().GetProperty("CorrelationId");
        return property?.GetValue(request)?.ToString();
    }

    private void SetCorrelationId(TRequest request, string correlationId)
    {
        var property = request.GetType().GetProperty("CorrelationId");
        property?.SetValue(request, correlationId);
    }
}

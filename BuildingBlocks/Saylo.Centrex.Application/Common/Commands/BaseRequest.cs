namespace Saylo.Centrex.Application.Common.Commands;

using MediatR;

public abstract class BaseRequest<TResponse> : IRequest<TResponse>
{
    public string? CorrelationId { get; set; }
}
public abstract class BaseRequest : IRequest
{
    public string? CorrelationId { get; set; }
}
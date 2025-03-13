using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Saylo.Centrex.Application.Common.Commands;

namespace Saylo.Centrex.Infrastructure.Web.Apis;
[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    protected ApiControllerBase(
        IMediator mediator,
        IMapper mapper,
        ILogger logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    protected async Task SendCommandAsync<TRequest, TModel>(
        TModel model,
        CancellationToken cancellationToken = default) 
        where TRequest : BaseRequest // Use a base request without generic type parameter
        where TModel : class
    {
        try
        {
            var command = _mapper.Map<TRequest>(model);
        
            _logger.LogInformation(
                "Sending command {CommandType} with correlation ID {CorrelationId}",
                typeof(TRequest).Name,
                command.CorrelationId);

            await _mediator.Send(command, cancellationToken);

            _logger.LogInformation(
                "Command {CommandType} processed successfully",
                typeof(TRequest).Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing command {CommandType}",
                typeof(TRequest).Name);
            throw;
        }
    }

    protected async Task<TResponse> SendCommandAsync<TRequest, TModel, TResponse>(
        TModel model,
        CancellationToken cancellationToken = default) 
        where TRequest : BaseRequest<TResponse> 
        where TModel : class
    {
        try
        {
            var command = _mapper.Map<TRequest>(model);
            
            _logger.LogInformation(
                "Sending command {CommandType} with correlation ID {CorrelationId}",
                typeof(TRequest).Name,
                command.CorrelationId);

            var response = await _mediator.Send<TResponse>(command, cancellationToken);

            _logger.LogInformation(
                "Command {CommandType} processed successfully",
                typeof(TRequest).Name);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing command {CommandType}",
                typeof(TRequest).Name);
            throw;
        }
    }

    protected async Task<TResponse> SendQueryAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default) 
        where TRequest : IRequest<TResponse> 
    {
        try
        {
            _logger.LogInformation(
                "Sending query {QueryType}",
                typeof(TRequest).Name);

            var response = await _mediator.Send(request, cancellationToken);

            _logger.LogInformation(
                "Query {QueryType} processed successfully",
                typeof(TRequest).Name);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error processing query {QueryType}",
                typeof(TRequest).Name);
            throw;
        }
    }
}
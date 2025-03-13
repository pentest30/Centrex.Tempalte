using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Saylo.Centrex.Application.Common.Commands;
using Saylo.Centrex.Domain.Repositories;
using Saylo.Centrex.Identity.Core.Domain.Entities;

namespace Saylo.Centrex.Identity.Core.Application.Commands.AdministrationDomains.ServiceProviders;

public class UpdateServiceProviderCommand : BaseRequest<bool>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string ServiceType { get; set; } 
    public string ContactEmail { get; set; }
}
public class UpdateServiceProviderCommandValidator : AbstractValidator<UpdateServiceProviderCommand>
{
    public UpdateServiceProviderCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.ServiceType)
            .NotEmpty().WithMessage("ServiceType is required.")
            .MaximumLength(50).WithMessage("ServiceType must not exceed 50 characters.");

        RuleFor(x => x.ContactEmail)
            .NotEmpty().WithMessage("ContactEmail is required.")
            .EmailAddress().WithMessage("ContactEmail must be a valid email address.");
    }
}
public class UpdateServiceProviderCommandHandler : IRequestHandler<UpdateServiceProviderCommand, bool>
{
    private readonly IRepository<ServiceProvider, Guid> _repository;
    private readonly ILogger<UpdateServiceProviderCommandHandler> _logger;

    public UpdateServiceProviderCommandHandler(
        IRepository<ServiceProvider, Guid> repository,
        ILogger<UpdateServiceProviderCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateServiceProviderCommand request, CancellationToken cancellationToken)
    {
        var serviceProvider = await _repository.GetAll().FirstOrDefaultAsync(sp => sp.Id == request.Id, cancellationToken);
        if (serviceProvider == null)
        {
            _logger.LogWarning("ServiceProvider with ID: {Id} not found.", request.Id);
            throw new KeyNotFoundException($"ServiceProvider with ID: {request.Id} not found.");
        }

        // Update entity
        serviceProvider.UpdateInfo(request.Name, request.Description, request.ServiceType, request.ContactEmail, request.CorrelationId);
        await _repository.UpdateAsync(serviceProvider, cancellationToken);
        await _repository.UnitOfWork.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("ServiceProvider with ID: {Id} updated successfully.", request.Id);
        return true;
    }
}

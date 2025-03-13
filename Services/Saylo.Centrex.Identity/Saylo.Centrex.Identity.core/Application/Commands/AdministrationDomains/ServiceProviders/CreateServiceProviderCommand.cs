using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Saylo.Centrex.Application.Common.Commands;
using Saylo.Centrex.Domain.Repositories;
using Saylo.Centrex.Identity.Core.Domain.Entities;

namespace Saylo.Centrex.Identity.Core.Application.Commands.AdministrationDomains.ServiceProviders;

public class CreateServiceProviderCommand : BaseRequest<Guid>
{
    public CreateServiceProviderCommand()
    {
    }

    public CreateServiceProviderCommand(
        string name,
        string? description,
        string serviceType,
        string contactEmail)
    {
        Name = name;
        Description = description;
        ServiceType = serviceType;
        ContactEmail = contactEmail;
    }

    public string Name { get; set; }
    public string? Description { get; set; }
    public string ServiceType { get; set; }
    public string ContactEmail { get; set; }
}

public class CreateServiceProviderCommandValidator : AbstractValidator<CreateServiceProviderCommand>
{
    private readonly IRepository<ServiceProvider, Guid> _repository;


    public CreateServiceProviderCommandValidator(IRepository<ServiceProvider, Guid> repository)
    {
        _repository = repository;
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(100)
            .WithMessage("Name must not exceed 100 characters.")
            .MustAsync(BeUniqueName)
            .WithMessage("A ServiceProvider with the same name already exists.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.ServiceType)
            .NotEmpty()
            .WithMessage("ServiceType is required.")
            .MaximumLength(50)
            .WithMessage("ServiceType must not exceed 50 characters.");

        RuleFor(x => x.ContactEmail)
            .NotEmpty()
            .WithMessage("ContactEmail is required.")
            .EmailAddress()
            .WithMessage("ContactEmail must be a valid email address.");
        
    }
    private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
    {
        var existing = await _repository
            .GetAll()
            .AsNoTracking()
            .Where(sp => sp.Name == name)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        return existing == null;
    }
}

public class CreateServiceProviderCommandHandler : IRequestHandler<CreateServiceProviderCommand, Guid>
{
    private readonly IRepository<ServiceProvider, Guid> _repository;

    public CreateServiceProviderCommandHandler(IRepository<ServiceProvider, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateServiceProviderCommand command, CancellationToken cancellationToken)
    {
        var serviceProvider = new ServiceProvider().CreateNewServiceProvider(command.Name, command.Description,
            command.ServiceType, command.ContactEmail, command.CorrelationId);
        await _repository.AddAsync(serviceProvider, cancellationToken);
        await _repository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return serviceProvider.Id;
    }
}

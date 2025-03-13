using FluentValidation;
using MediatR;
using Saylo.Centrex.Application.Common.Commands;
using Saylo.Centrex.Domain.Entities.ValueObjects;
using Saylo.Centrex.Domain.Repositories;
using Saylo.Centrex.Identity.Core.Domain.Entities;

namespace Saylo.Centrex.Identity.Core.Application.Commands.AdministrationDomains.Enterprises;

public class CreateEnterpriseCommand : BaseRequest<Guid>
{
    public CreateEnterpriseCommand()
    {
    }
    public CreateEnterpriseCommand(
        string name,
        string? description,
        string? siret,
        Address mainAddress,
        Address? secondAddress)
    {
        Name = name;
        Description = description;
        Siret = siret;
        MainAddress = mainAddress;
        SecondAddress = secondAddress;
    }
    public string Name { get; set; } 
    public string? Description { get; set; } 
    public string? Siret { get; set; } 
    public Address MainAddress { get; set; }
    public Address? SecondAddress { get; set; } 
}

public class CreateEnterpriseCommandValidator : AbstractValidator<CreateEnterpriseCommand>
{
    public CreateEnterpriseCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.Siret)
            .Matches(@"^\d{14}$").When(x => !string.IsNullOrEmpty(x.Siret))
            .WithMessage("Siret must be a 14-digit number.");

        RuleFor(x => x.MainAddress)
            .NotNull().WithMessage("MainAddress is required.")
            .SetValidator(new AddressValidator());
        RuleFor(x => x.SecondAddress)
            .SetValidator(new AddressValidator()).When(x => x.SecondAddress != null);
    }
}

public class CreateEnterpriseCommandHandler : IRequestHandler<CreateEnterpriseCommand, Guid>
{
    private readonly IRepository<Enterprise, Guid> _repository;

    public CreateEnterpriseCommandHandler(IRepository<Enterprise, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateEnterpriseCommand command, CancellationToken cancellationToken)
    {
        var enterprise = new Enterprise().CreateNewEnterprise(command.Name, command.Description, command.Siret, command.MainAddress, command.SecondAddress, command.CorrelationId);
        await _repository.AddAsync(enterprise, cancellationToken);
        await _repository.UnitOfWork.SaveChangesAsync(cancellationToken);
        return enterprise.Id;
    }
}
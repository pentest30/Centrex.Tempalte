using Saylo.Centrex.Domain.Entities;
using Saylo.Centrex.Domain.Entities.ValueObjects;
using Saylo.Centrex.Identity.Core.Domain.DomainEvents.AdministrationDomains;

namespace Saylo.Centrex.Identity.Core.Domain.Entities;

public class Enterprise : AggregateRoot<Guid>, ITenantEntity
{
    public Enterprise()
    {
    }

    private Enterprise(string name, string? description, string siret, Address mainAddress, Address? secondAddress)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Siret = siret;
        MainAddress = mainAddress;
        SecondAddress = secondAddress;
    }
    public Enterprise CreateNewEnterprise(string name, string? description, string siret, Address mainAddress,
        Address? secondAddress, string correlationId)
    {
        var entreprise = new Enterprise(name, description, siret, mainAddress, secondAddress);
        entreprise.AddDomainEvent(new EnterpriseCreatedEvent(entreprise, correlationId));
        return entreprise;
    }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string Siret { get; private set; }
    public Address MainAddress { get; private set; }
    public Address? SecondAddress { get; private set; }
    public Guid? TenantId { get; set; }
}


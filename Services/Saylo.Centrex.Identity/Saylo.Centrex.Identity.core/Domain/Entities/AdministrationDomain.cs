using Saylo.Centrex.Domain.Entities;
using Saylo.Centrex.Identity.Core.Domain.DomainEvents.AdministrationDomains;
using Saylo.Centrex.Identity.Core.Domain.Entities.Identity;

namespace Saylo.Centrex.Identity.Core.Domain.Entities;

public class AdministrationDomain : TenantInfoBase, ITenantEntity
{
    public AdministrationDomain()
    {
    }
    public string? Description { get; private set; }
    // Propriété discriminante
    public TypeEntity TypeEntity { get; set; }
    public ICollection<AdministrationDomain> Children { get; private set; }
    public ICollection<ApplicationUser> Users { get; private set; }
    public ICollection<Functionality> Functionalities { get; private set; }
    public AdministrationDomain Tenant { get; private set; }
    public Guid? TenantId { get; set; }

    public AdministrationDomain CreateNewTenantFormEntreprise(Enterprise enterprise, string correlationId)
    {
        var administrationDomain = new AdministrationDomain
        {
            Id = enterprise.Id,
            Name = enterprise.Name,
            Description = enterprise.Description,
            TypeEntity = TypeEntity.Entreprise,
            IsActive = true
        };

        administrationDomain.AddDomainEvent(new AdministrationDomainCreatedEvent(administrationDomain, correlationId));
        return administrationDomain;
    }
    public AdministrationDomain CreateNewTenantFormServiceProvider(ServiceProvider serviceProvider, string correlationId)
    {
        var administrationDomain = new AdministrationDomain
        {
            Id = serviceProvider.Id,
            Name = serviceProvider.Name,
            Description = serviceProvider.Description,
            TypeEntity = TypeEntity.ServiceProvider,
            Email = serviceProvider.ContactEmail,
            IsActive = true
        };

        administrationDomain.AddDomainEvent(new AdministrationDomainCreatedEvent(administrationDomain, correlationId));
        return administrationDomain;
    }
    public void UpdateInfo(string providerName, string? providerDescription, string providerContactEmail)
    {
        Name = providerName;
        Description = providerDescription;
        Email = providerContactEmail;
    }
}
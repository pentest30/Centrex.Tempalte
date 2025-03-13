using System.Linq.Expressions;
using Saylo.Centrex.Application.Common.ManualMapping;
using Saylo.Centrex.Identity.Core.Domain.Entities;

namespace Saylo.Centrex.Identity.Core.Application.DTOs;

public class AdministrationDomainDto : IMapFrom<AdministrationDomain>
{
    public Expression<Func<AdministrationDomain, object>> Mapping()
    {
        return administrationDomain => new AdministrationDomainDto
        {
            Id = administrationDomain.Id,
            Name = administrationDomain.Name,
            TenantId = administrationDomain.TenantId,
            Email = administrationDomain.Email,
            IsActive = administrationDomain.IsActive,
            EntityType = administrationDomain.TypeEntity.ToString()
        };
    }

    public string EntityType { get; set; }
    
    public bool IsActive { get; set; }

    public string? Email { get; set; }

    public Guid? TenantId { get; set; }

    public string Name { get; set; }

    public Guid Id { get; set; }
    
}
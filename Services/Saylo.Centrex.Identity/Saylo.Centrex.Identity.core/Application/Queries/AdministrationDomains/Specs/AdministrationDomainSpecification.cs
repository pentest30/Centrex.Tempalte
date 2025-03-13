using Microsoft.EntityFrameworkCore;
using Saylo.Centrex.Application.Common.Specifications;
using Saylo.Centrex.Application.Extensions;
using Saylo.Centrex.Domain.Specifications;
using Saylo.Centrex.Identity.Core.Application.DTOs;
using Saylo.Centrex.Identity.Core.Domain.Entities;

namespace Saylo.Centrex.Identity.Core.Application.Queries.AdministrationDomains.Specs;

public class AdministrationDomainSpecification : SpecificationBuilder<AdministrationDomain, AdministrationDomainDto>
{
    public AdministrationDomainSpecification(IProjectionProvider projectionProvider,
        List<Guid> tenantIds,
        string? searchTerm,
        int page,
        int size) : base(projectionProvider)
    {

        Where(x => tenantIds.Contains(x.Id));
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            Where(x => x.Name.Contains(searchTerm) || (x.Email != null && x.Email.Contains(searchTerm)));
        }

        Include(x => x.Children);
        this.Paging(page, size);
        IgnoreQueryFilters();
        AsNoTracking();
    }
}

public class AdministrationDomainCountSpecification : SpecificationBuilder<AdministrationDomain>
{
    public AdministrationDomainCountSpecification(
        List<Guid> tenantIds,
        string? searchTerm)
    {

        Where(x => tenantIds.Contains(x.Id));
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            Where(x => x.Name.Contains(searchTerm) ||
                       (x.Email != null && x.Email.Contains(searchTerm)));
        }

        IgnoreQueryFilters();
        AsNoTracking();
    }
}
public class AdministrationDomainByTenantIdSpecification : SpecificationBuilder<AdministrationDomain, Guid>
{
    public AdministrationDomainByTenantIdSpecification(
        IProjectionProvider projectionProvider,
        Guid parentId,
        int page,
        int size) : base(projectionProvider)
    {
        Where(x => x.TenantId == parentId);
        Select(x => x.Id);
        OrderBy(x => x.Id);
        this.Paging(page, size);
        AsNoTracking();
        IgnoreQueryFilters();
    }
}
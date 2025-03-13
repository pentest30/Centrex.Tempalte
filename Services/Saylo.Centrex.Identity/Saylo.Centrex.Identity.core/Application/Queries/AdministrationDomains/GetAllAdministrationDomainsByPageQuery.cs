using MediatR;
using Saylo.Centrex.Application.Common.Queries;
using Saylo.Centrex.Application.Extensions;
using Saylo.Centrex.Application.Multitenancy;
using Saylo.Centrex.Domain.Repositories;
using Saylo.Centrex.Domain.Specifications;
using Saylo.Centrex.Identity.Core.Application.DTOs;
using Saylo.Centrex.Identity.Core.Application.Queries.AdministrationDomains.Specs;
using Saylo.Centrex.Identity.Core.Domain.Entities;

namespace Saylo.Centrex.Identity.Core.Application.Queries.AdministrationDomains;

public class GetAllAdministrationDomainsByPageQuery : IRequest<PagedResult<AdministrationDomainDto>>
{
    public string? Search { get; set; }
    public int Page { get; set; }
    public int Size { get; set; }
}
public class GetAllAdministrationDomainsByPageQueryHandler 
    : IRequestHandler<GetAllAdministrationDomainsByPageQuery, PagedResult<AdministrationDomainDto>>
{
    private readonly IReadRepository<AdministrationDomain> _repository;
    private readonly IProjectionProvider _projectionProvider;
    private readonly ITenantContextAccessor _tenantContextAccessor;

    public GetAllAdministrationDomainsByPageQueryHandler(
        IReadRepository<AdministrationDomain> repository,
        IProjectionProvider projectionProvider,
        ITenantContextAccessor tenantContextAccessor)
    {
        _repository = repository;
        _projectionProvider = projectionProvider;
        _tenantContextAccessor = tenantContextAccessor;
    }

    public async Task<PagedResult<AdministrationDomainDto>> Handle(GetAllAdministrationDomainsByPageQuery request,
        CancellationToken cancellationToken)
    {
        var tenantIds = await TraversTenantChildrenAsync(
            request.Page,
            request.Size);
        var spec = new AdministrationDomainSpecification(
                _projectionProvider,
                tenantIds,
                request.Search,
                request.Page,
                request.Size)
            .Project()
            .Build();

        var countSpec = new AdministrationDomainCountSpecification(
                tenantIds,
                request.Search)
            .Build();

        return await _repository.ToPagedResultAsync(spec, countSpec, cancellationToken);
    }

    private async Task<List<Guid>> TraversTenantChildrenAsync(int page , int size )
    {
        var ids = new List<Guid> { _tenantContextAccessor.TenantId.Value };
        await LoadChildrenAsync(ids, _tenantContextAccessor.TenantId.Value, page, size);
        return ids;
    }

    private async Task LoadChildrenAsync(List<Guid> ids, Guid parentId , int page , int size )
    {
        var spec = new AdministrationDomainByTenantIdSpecification(_projectionProvider, parentId, page, size)
            .Build();

        var childrenIds = await _repository.ListAsync(spec);

        if (childrenIds.Any())
        {
            ids.AddRange(childrenIds); 
            
            foreach (var childId in childrenIds)
            {
                await LoadChildrenAsync(ids, childId, page, size);
            }
        }
    }
}

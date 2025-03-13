using MediatR;
using Saylo.Centrex.Application.Common.Specifications;
using Saylo.Centrex.Application.Multitenancy;
using Saylo.Centrex.Domain.Repositories;
using Saylo.Centrex.Domain.Specifications;
using Saylo.Centrex.Identity.Core.Application.DTOs;
using Saylo.Centrex.Identity.Core.Domain.Entities;

namespace Saylo.Centrex.Identity.Core.Application.Queries.AdministrationDomains;

public class GetAdministrationDomainByIdQuery : IRequest<AdministrationDomainDto>;

public class GetAdministrationDomainByIdQueryHandler : IRequestHandler<GetAdministrationDomainByIdQuery, AdministrationDomainDto>
{
    private readonly IReadRepository<AdministrationDomain> _repository;
    private readonly IProjectionProvider _projectionProvider;
    private readonly ITenantContextAccessor _tenantContextAccessor;

    public GetAdministrationDomainByIdQueryHandler(
        IReadRepository<AdministrationDomain> repository,
        IProjectionProvider projectionProvider,
        ITenantContextAccessor tenantContextAccessor)
    {
        _repository = repository;
        _projectionProvider = projectionProvider;
        _tenantContextAccessor = tenantContextAccessor;
    }

    public async Task<AdministrationDomainDto> Handle(GetAdministrationDomainByIdQuery request,
        CancellationToken cancellationToken)
    {
        var spec = new SpecificationBuilder<AdministrationDomain, AdministrationDomainDto>(_projectionProvider)
            .Where(t => t.Id == _tenantContextAccessor.TenantId)
            .Project()
            .IgnoreQueryFilters()
            .Build();
        return await _repository.FirstOrDefaultAsync(spec, cancellationToken);
    }
}

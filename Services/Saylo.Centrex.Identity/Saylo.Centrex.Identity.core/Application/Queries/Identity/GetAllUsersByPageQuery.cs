using AutoMapper;
using MediatR;
using Saylo.Centrex.Application.Common.Queries;
using Saylo.Centrex.Application.Common.Specifications;
using Saylo.Centrex.Application.Extensions;
using Saylo.Centrex.Domain.Repositories;
using Saylo.Centrex.Domain.Specifications;
using Saylo.Centrex.Identity.Core.Application.DTOs;
using Saylo.Centrex.Identity.Core.Domain.Entities.Identity;

namespace Saylo.Centrex.Identity.Core.Application.Queries.Identity;
public class GetAllUsersByPageQuery : IRequest<PagedResult<UserDto>>
{
    public int Page { get; set; }
    public int Size { get; set; } 
}
public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersByPageQuery, PagedResult<UserDto>>
{
    private readonly IReadRepository<ApplicationUser> _readRepository;
    private readonly IProjectionProvider _projectionProvider;
    public GetAllUsersQueryHandler(IReadRepository<ApplicationUser> readRepository, IMapper mapper, IProjectionProvider projectionProvider)
    {
        _readRepository = readRepository;
        _projectionProvider = projectionProvider;
    }
    public async Task<PagedResult<UserDto>> Handle(GetAllUsersByPageQuery request, CancellationToken cancellationToken)
    {
        var spec = GetSpecification(request);
        var countSpec = CountSpec();
        return await _readRepository.ToPagedResultAsync(spec, countSpec, cancellationToken);
    }

    private static ISpecification<ApplicationUser> CountSpec()
    {
        var countSpec = new SpecificationBuilder<ApplicationUser>()
            .Build();
        return countSpec;
    }
    private ISpecification<ApplicationUser, UserDto> GetSpecification(GetAllUsersByPageQuery request)
    {
        return new SpecificationBuilder<ApplicationUser, UserDto>(_projectionProvider)
            .Include(x=>x.UserRoles)
            .IncludeString("UserRoles.Role")
            .Paging(request.Page, request.Size)
            .OrderBy(x=>x.UserName)
            .Project()
            .Build();
    }
}

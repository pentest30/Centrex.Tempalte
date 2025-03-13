using MediatR;
using Saylo.Centrex.Application.Common.Specifications;
using Saylo.Centrex.Domain.Repositories;
using Saylo.Centrex.Domain.Specifications;
using Saylo.Centrex.Identity.Core.Application.DTOs;
using Saylo.Centrex.Identity.Core.Domain.Entities.Identity;

namespace Saylo.Centrex.Identity.Core.Application.Queries.Identity;

public class GetUserByIdQuery : IRequest<UserDto>
{
    public Guid UserId { get; set; }
}
public class GetUserByIdQueryHandler(IReadRepository<ApplicationUser> identityService, IProjectionProvider projectionProvider)
    : IRequestHandler<GetUserByIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var spec = GetSpecification(request.UserId);
        return await identityService.FirstOrDefaultAsync(spec, cancellationToken);
    }
    
    private ISpecification<ApplicationUser, UserDto> GetSpecification(Guid userId)
    {
        return new SpecificationBuilder<ApplicationUser, UserDto>(projectionProvider)
            .Include(x=>x.UserRoles)
            .IncludeString("UserRoles.Role")
            .Where(u=>u.Id == userId)
            .Project()
            .Build();
    }
}
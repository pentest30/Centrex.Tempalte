using System.Linq.Expressions;
using Saylo.Centrex.Application.Common.ManualMapping;
using Saylo.Centrex.Identity.Core.Domain.Entities.Identity;

namespace Saylo.Centrex.Identity.Core.Application.DTOs;

public class UserDto : IMapFrom<ApplicationUser>
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
    public string UserType { get; set; }
    public Guid? TenantId { get; set; }
    public List<string> Roles { get; set; }
    public Expression<Func<ApplicationUser, object>> Mapping()
    {
        return user => new UserDto
        {
            Id = user.Id.ToString(),
            Email = user.Email,
            Roles = user.UserRoles.Select(x=>x.Role.Name).ToList(),
            IsActive = user.IsActive,
            TenantId = user.TenantId,
            UserName = user.UserName,
            UserType = user.TypeUser.ToString()
        };
    }
}
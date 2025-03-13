using Microsoft.AspNetCore.Identity;

namespace Saylo.Centrex.Identity.Core.Domain.Entities.Identity;

public class AdminRole : IdentityRole<Guid>
{
    public AdminRole()
    {
    }
    public AdminRole(string roleName) : base(roleName)
    {
    }
    public virtual ICollection<AdminUserRole> UserRoles { get; set; }
    public virtual ICollection<AdminRoleClaim> RoleClaims { get; set; }
}
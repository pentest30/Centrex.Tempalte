using Microsoft.AspNetCore.Identity;

namespace Saylo.Centrex.Identity.Core.Domain.Entities.Identity;

public class AdminUserRole : IdentityUserRole<Guid>
{
    public virtual ApplicationUser User { get; set; }
    public virtual AdminRole Role { get; set; }
}
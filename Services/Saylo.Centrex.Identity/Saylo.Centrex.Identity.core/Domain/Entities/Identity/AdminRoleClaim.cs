using Microsoft.AspNetCore.Identity;

namespace Saylo.Centrex.Identity.Core.Domain.Entities.Identity;

public class AdminRoleClaim : IdentityRoleClaim<Guid>
{
    public virtual AdminRole Role { get; set; }

}
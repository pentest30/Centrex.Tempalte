using Saylo.Centrex.Identity.Core.Application.Commands.Identity;
using Saylo.Centrex.Identity.Core.Application.Models;
using Saylo.Centrex.Identity.Core.Domain.Entities.Identity;

namespace Saylo.Centrex.Identity.Core.Application.Interfaces;

public interface IIdentityService
{
    Task<ApplicationUser> GetUserByUserNameAsync(string userName);
    Task<IdentityResponse> RegisterAsync(CreateUserCommande request);
    Task<ApplicationUser> GetUserByIdAsync(string userId, bool includeRoles = false, bool ignoreTenantFilter = false);
    Task<bool> AssignRoleAsync(Guid userId, string roleName);
    Task<bool> RemoveRoleAsync(string userId, string roleName);
    Task<bool> DeactivateUserAsync(string userId);
    Task<bool> RemoveUserAsync(string userId);
    Task<IEnumerable<ApplicationUser>> GetUsersByTenantAsync(bool includeRoles = false);
    Task<ApplicationUser?> GetUserByEmailAsync(string email);
}
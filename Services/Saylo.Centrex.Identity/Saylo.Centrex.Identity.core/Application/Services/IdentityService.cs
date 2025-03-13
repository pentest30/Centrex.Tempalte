using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Saylo.Centrex.Application.Exceptions;
using Saylo.Centrex.Identity.Core.Application.Commands.Identity;
using Saylo.Centrex.Identity.Core.Application.Interfaces;
using Saylo.Centrex.Identity.Core.Application.Models;
using Saylo.Centrex.Identity.Core.Domain.Entities.Identity;
using Saylo.Centrex.Identity.Core.Infrastructure.Persistence;
using Saylo.Centrex.Application.Multitenancy;

namespace Saylo.Centrex.Identity.Core.Application.Services;

public class IdentityService : IIdentityService
{
    private readonly SayloIdentityDbContext _context;
    private readonly ILogger<IdentityService> _logger;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly ITenantContextAccessor _tenantContextAccessor;

    public IdentityService(
        SayloIdentityDbContext context,
        IPasswordHasher<ApplicationUser> passwordHasher,
        ILogger<IdentityService> logger,
        ITenantContextAccessor tenantContextAccessor)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _logger = logger;
        _tenantContextAccessor = tenantContextAccessor;
    }

    public async Task<ApplicationUser> GetUserByUserNameAsync(string userName)
    {
        try
        {
            // Ignore tenant filter for login
            var query = _context.Users.AsQueryable();
            return await query
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.UserName == userName && u.IsActive);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user for login with email {Email}", userName);
            throw;
        }
    }

    public async Task<IdentityResponse> RegisterAsync(CreateUserCommande request)
    {
        try
        {
            if (request.Password != request.ConfirmPassword)
                return IdentityResponse.Failure( "Passwords do not match");
            
            var user = new ApplicationUser(
                request.UserName, 
                request.Email, 
                string.Empty, // Temporary password hash
                request.TypeUser, 
                request.CorrelationId);

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            user.SecurityStamp = Guid.NewGuid().ToString();
            user.ConcurrencyStamp = Guid.NewGuid().ToString();
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == request.TypeUser.ToString());

            if (role == null)
            {
                role = new AdminRole(request.TypeUser.ToString());
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
            }

            user.UserRoles = new List<AdminUserRole> 
            { 
                new() { UserId = user.Id, RoleId = role.Id } 
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} registered for tenant {TenantId}", 
                user.Id, user.TenantId);

            return IdentityResponse.Success("User registered successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            throw;
        }
    }

    public async Task<ApplicationUser> GetUserByIdAsync(string userId, bool includeRoles = false, bool ignoreTenantFilter = false)
    {
        try
        {
            var query = _context.Users.AsQueryable();

            if (ignoreTenantFilter)
            {
                query = query.IgnoreQueryFilters();
            }

            if (includeRoles)
            {
                query = query
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role);
            }

            var user = await query
                .FirstOrDefaultAsync(u => u.Id.ToString() == userId && u.IsActive);
            
            if (user == null) 
                throw new NotFoundException($"User not found");

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> AssignRoleAsync(Guid userId, string roleName)
    {
        try
        {   
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive);

            if (user == null)
                return false;

            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == roleName);

            if (role == null)
            {
                role = new AdminRole(roleName);
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
            }

            if (user.UserRoles.All(ur => ur.RoleId != role.Id))
            {
                user.UserRoles.Add(new AdminUserRole { UserId = user.Id, RoleId = role.Id });
                await _context.SaveChangesAsync();

                _logger.LogInformation("Role {RoleName} assigned to user {UserId}", 
                    roleName, userId);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {RoleName} to user {UserId}", 
                roleName, userId);
            throw;
        }
    }

    public async Task<bool> RemoveRoleAsync(string userId, string roleName)
    {
        try
        {
            var userRole = await _context.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync(ur => 
                    ur.User.Id.ToString() == userId && 
                    ur.Role.Name == roleName &&
                    ur.User.IsActive);

            if (userRole == null)
                return true;

            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Role {RoleName} removed from user {UserId}", 
                roleName, userId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role {RoleName} from user {UserId}", 
                roleName, userId);
            throw;
        }
    }

    public async Task<bool> DeactivateUserAsync(string userId)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id.ToString() == userId && u.IsActive);

            if (user == null)
                return false;

            user.LockoutEnd = DateTimeOffset.MaxValue;
            user.IsActive = false;
            
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} deactivated", userId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating user {UserId}", userId);
            throw;
        }
    }

    public async Task<bool> RemoveUserAsync(string userId)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id.ToString() == userId && u.IsActive);

            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} removed", userId);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing user {UserId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<ApplicationUser>> GetUsersByTenantAsync(bool includeRoles = false)
    {
        try
        {
            var query = _context.Users.Where(u => u.IsActive);

            if (includeRoles)
            {
                query = query
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role);
            }

            var users = await query.ToListAsync();
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users for tenant {TenantId}", 
                _tenantContextAccessor.TenantId);
            throw;
        }
    }
    public Task<ApplicationUser?> GetUserByEmailAsync(string email)
    {
        return _context.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
    }
}
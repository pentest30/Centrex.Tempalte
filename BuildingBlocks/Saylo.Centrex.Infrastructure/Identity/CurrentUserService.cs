using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;
using Saylo.Centrex.Application.Common.Identity;

namespace Saylo.Centrex.Infrastructure.Identity;
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string ConnectionId { get; set; }

    public string UserId => GetClaim(OpenIddictConstants.Claims.Subject) ?? string.Empty;
    
    public string UserName => GetClaim(OpenIddictConstants.Claims.Name) ?? string.Empty;
    
    public string Email => GetClaim(OpenIddictConstants.Claims.Email) ?? string.Empty;
    public string TenantId => GetClaim("tenantId") ?? string.Empty;

    public IEnumerable<string> Roles => User?.Claims
        .Where(c => c.Type == OpenIddictConstants.Claims.Role)
        .Select(c => c.Value) ?? Enumerable.Empty<string>();
    
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public string? GetClaim(string claimType)
    {
        return User?.Claims
            .FirstOrDefault(c => c.Type == claimType)
            ?.Value;
    }

    public bool HasRole(string role)
    {
        return Roles.Any(r => r.Equals(role, StringComparison.OrdinalIgnoreCase));
    }

    public bool HasClaim(string claimType, string claimValue)
    {
        return User?.Claims.Any(c => 
            c.Type == claimType && 
            c.Value.Equals(claimValue, StringComparison.OrdinalIgnoreCase)) ?? false;
    }
}
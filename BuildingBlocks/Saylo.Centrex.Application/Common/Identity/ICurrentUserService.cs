namespace Saylo.Centrex.Application.Common.Identity;

public interface ICurrentUserService
{
    string UserId { get; }
    string UserName { get; }
    string Email { get; }
    public string TenantId { get; }
    public string ConnectionId { get; set; }
    IEnumerable<string> Roles { get; }
    bool IsAuthenticated { get; }
    string? GetClaim(string claimType);
    bool HasRole(string role);
    bool HasClaim(string claimType, string claimValue);
}

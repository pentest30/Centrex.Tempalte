using Saylo.Centrex.Application.Common.Identity;

namespace Saylo.Centrex.Infrastructure.Identity;
public class CurrentUser
{
    public CurrentUser()
    {

    }

    public CurrentUser(ICurrentUserService currentUserService)
    {
        UserId = currentUserService.UserId;
        UserName = currentUserService.UserName;
        Email = currentUserService.Email;
        TenantId = currentUserService.TenantId;
        ConnectionId = currentUserService.ConnectionId;
        Roles = currentUserService.Roles;
        IsAuthenticated = currentUserService.IsAuthenticated;
    }

    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string TenantId { get; set; }
    public string ConnectionId { get; set; }
    public IEnumerable<string> Roles { get; set; }
    public bool IsAuthenticated { get; set; }
}
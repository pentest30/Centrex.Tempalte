using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;

namespace Saylo.Centrex.Infrastructure.Workers.Hangfire;

public class DashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly string _user;
    private readonly string _password;

    public DashboardAuthorizationFilter(string user, string password)
    {
        _user = user;
        _password = password;
    }

    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        var authHeader = httpContext.Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Basic "))
        {
            var encodedCredentials = authHeader["Basic ".Length..].Trim();
            var credentials = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials)).Split(':');
            return credentials[0] == _user && credentials[1] == _password;
        }

        httpContext.Response.Headers["WWW-Authenticate"] = "Basic";
        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return true;
    }
}
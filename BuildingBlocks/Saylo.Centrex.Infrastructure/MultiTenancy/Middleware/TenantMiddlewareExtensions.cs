using Microsoft.AspNetCore.Http;

namespace Saylo.Centrex.Infrastructure.MultiTenancy.Middleware;

public static class TenantMiddlewareExtensions
{
    private static readonly HashSet<string> WellKnownPaths = new()
    {
        "/connect/token",
        "/connect/authorize",
        "/connect/userinfo",
        "/connect/endsession",
        "/connect/introspect",
        "/connect/logout",
        "/api/login",
        "/chat/chat-hub",
        "/.well-known/openid-configuration",
        "/swagger",
        "/swagger/index.html",
        "/swagger/v1/swagger.json",
        "/swagger/v2/swagger.json",
        "/swagger/oauth2-redirect.html",
        "/favicon.ico",
        "/health",
        "/_framework"
    };

    public static bool IsWellKnownEndpoint(PathString path)
    {
        if (string.IsNullOrEmpty(path.Value))
            return false;

        // First check if the path matches directly
        if (WellKnownPaths.Any(allowedPath => 
                path.Value.StartsWith(allowedPath, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        // Check if path follows proxy pattern (/{service}/...)
        var segments = path.Value.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length >= 2)
        {
            // Remove the first segment (service name) and check the remaining path
            var pathWithoutPrefix = "/" + string.Join('/', segments.Skip(1));
            return WellKnownPaths.Any(allowedPath => 
                pathWithoutPrefix.StartsWith(allowedPath, StringComparison.OrdinalIgnoreCase));
        }

        return false;
    }
}
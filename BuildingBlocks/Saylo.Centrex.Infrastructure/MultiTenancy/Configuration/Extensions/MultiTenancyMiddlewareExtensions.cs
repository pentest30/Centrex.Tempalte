using Microsoft.AspNetCore.Builder;
using Saylo.Centrex.Infrastructure.MultiTenancy.Middleware;

namespace Saylo.Centrex.Infrastructure.MultiTenancy.Configuration.Extensions;

public static class MultiTenancyMiddlewareExtensions
{
    public static IApplicationBuilder UseMultiTenancy(this IApplicationBuilder app)
    {
        return app.UseMiddleware<TenantMiddleware>();
    }
}
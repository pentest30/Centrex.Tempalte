using Microsoft.AspNetCore.Http;
using Saylo.Centrex.Application.Multitenancy;
using Saylo.Centrex.Infrastructure.MultiTenancy.Configuration;

namespace Saylo.Centrex.Infrastructure.MultiTenancy.Middleware.TenantResolvers;

public class HeaderTenantResolver : ITenantResolver
{
    private readonly string _headerName;

    public HeaderTenantResolver(MultiTenantConfig tenantConfig)
    {
        _headerName = tenantConfig.TenantHeaderKey;
    }

    public Task<string?> ResolveTenantIdAsync(HttpContext context)
    {
        context.Request.Headers.TryGetValue(_headerName, out var tenantId);
        return Task.FromResult(tenantId.FirstOrDefault());
    }
}

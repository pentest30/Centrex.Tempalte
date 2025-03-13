using Microsoft.AspNetCore.Http;
using Saylo.Centrex.Application.Multitenancy;

namespace Saylo.Centrex.Infrastructure.MultiTenancy.Middleware.TenantResolvers;

public class ParamQueryTenantResolver : ITenantResolver
{
    private readonly string _queryParamName;

    public ParamQueryTenantResolver(string queryParamName = "tenantId")
    {
        _queryParamName = queryParamName;
    }

    public Task<string?> ResolveTenantIdAsync(HttpContext context)
    {
        context.Request.Query.TryGetValue(_queryParamName, out var tenantId);
        return Task.FromResult(tenantId.FirstOrDefault());
    }
}

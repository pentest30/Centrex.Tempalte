using Microsoft.AspNetCore.Http;

namespace Saylo.Centrex.Application.Multitenancy;

public interface ITenantResolver
{
    Task<string?> ResolveTenantIdAsync(HttpContext context);
}
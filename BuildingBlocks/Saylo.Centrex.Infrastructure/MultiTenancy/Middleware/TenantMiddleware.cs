using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Saylo.Centrex.Application.Multitenancy;

namespace Saylo.Centrex.Infrastructure.MultiTenancy.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    public TenantMiddleware(
        RequestDelegate next,
        IServiceProvider serviceProvider)
    {
        _next = next;
        _serviceProvider = serviceProvider;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (TenantMiddlewareExtensions.IsWellKnownEndpoint(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var allowAnonymous = IsAllowedWithoutTenant(context);
        if (!allowAnonymous)
        {
            using var scope = _serviceProvider.CreateScope();
            var tenantResolver = scope.ServiceProvider.GetRequiredService<ITenantResolver>();
            var tenantContextAccessor = scope.ServiceProvider.GetRequiredService<ITenantContextAccessor>();

            tenantContextAccessor.TenantId = null;

            var tenantId = await tenantResolver.ResolveTenantIdAsync(context);
            if (string.IsNullOrEmpty(tenantId))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Tenant identifier is required.");
                return;
            }

            tenantContextAccessor.TenantId = Guid.Parse(tenantId);
        }

        await _next(context);
    }


    static bool IsAllowedWithoutTenant(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint == null)
        {
            var endpointFeature = context.Features.Get<IEndpointFeature>();
            endpoint = endpointFeature?.Endpoint;
        }

        return endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null;
    }
}



using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Saylo.Centrex.Application.Common.Identity;
using Saylo.Centrex.Application.Multitenancy;

namespace Saylo.Centrex.Infrastructure.MultiTenancy.Middleware;

public class TenantAuthorizationMiddleware
{
    private readonly RequestDelegate _next;

    public TenantAuthorizationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
    {
        // If it's a well-known endpoint, skip tenant validation and continue the pipeline
        if (TenantMiddlewareExtensions.IsWellKnownEndpoint(context.Request.Path))
        {
            await _next(context);
            return;
        }

        using var scope = serviceProvider.CreateScope();
        var tenantCache = scope.ServiceProvider.GetRequiredService<ITenantCacheStore>();
        var tenantResolver = scope.ServiceProvider.GetRequiredService<ITenantResolver>();

        var tenantId = await tenantResolver.ResolveTenantIdAsync(context);

        if (!IsAuthorized(tenantId))
        {
            await WriteErrorResponseAsync(context, 401, "User is not authenticated or tenant ID is missing.");
            return;
        }

        var hasAccess = await tenantCache.ValidateUserTenantAccessAsync(tenantId);
        if (!hasAccess)
        {
            await WriteErrorResponseAsync(context, 403, "Access to the specified tenant is forbidden.");
            return;
        }

        await _next(context);
    }

    private static bool IsAuthorized(string tenantId) =>
        !string.IsNullOrEmpty(tenantId);

    private static async Task WriteErrorResponseAsync(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            Status = statusCode,
            Message = message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Saylo.Centrex.Application.Multitenancy;

namespace Saylo.Centrex.Infrastructure.MultiTenancy.Middleware.TenantResolvers
{
    public class TenantResolverPipeline : ITenantResolver
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TenantResolverPipeline> _logger;

        public TenantResolverPipeline(IServiceProvider serviceProvider, ILogger<TenantResolverPipeline> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task<string?> ResolveTenantIdAsync(HttpContext context)
        {
            var resolvers = _serviceProvider.GetServices<ITenantResolver>()
                .Where(resolver => resolver.GetType() != typeof(TenantResolverPipeline)); // Exclude self

            foreach (var resolver in resolvers)
            {
                var tenantId = await resolver.ResolveTenantIdAsync(context);
                if (!string.IsNullOrEmpty(tenantId))
                {
                    _logger.LogInformation("Tenant resolved by {Resolver}: {TenantId}", resolver.GetType().Name, tenantId);
                    return tenantId;
                }
            }

            _logger.LogWarning("No tenant was resolved.");
            return null; // Or throw an exception if a tenant is required
        }
    }
}
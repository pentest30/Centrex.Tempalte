using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Saylo.Centrex.Domain.Entities;
using Saylo.Centrex.Infrastructure.MultiTenancy.Middleware.TenantResolvers;

namespace Saylo.Centrex.Infrastructure.MultiTenancy.Configuration.Extensions;

public static class MultiTenancyExtension
{
    public static MultiTenantBuilder ConfigureMultiTenancy<TContext>(
        this IServiceCollection services,
        IConfiguration configuration,
        string? migrationsAssembly = null)
        where TContext : DbContext
    {
        var multiTenantConfig = configuration.GetSection(nameof(MultiTenantConfig)).Get<MultiTenantConfig>();
        if (multiTenantConfig == null)
        {
            throw new ArgumentNullException(nameof(MultiTenantConfig),
                "MultiTenantConfig section is missing or invalid.");
        }

        services.AddSingleton(multiTenantConfig);
        var multiTenantBuilder = new MultiTenantBuilder(services, configuration);
        multiTenantBuilder
            .AddTenantResolvers(typeof(HeaderTenantResolver), typeof(ParamQueryTenantResolver)) // Add resolvers
            .AddMultiTenantDbContext<TContext>(migrationsAssembly) // Configure DbContext
            .AddDefaultTenant(
                Guid.Parse(multiTenantConfig.DefaultTenantId),
                multiTenantConfig.DefaultConnectionString);
        return multiTenantBuilder;
    }
}
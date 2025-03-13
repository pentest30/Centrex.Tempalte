using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Saylo.Centrex.Application.Multitenancy;
using Saylo.Centrex.Domain.Entities;
using Saylo.Centrex.Infrastructure.MultiTenancy.Core;
using Saylo.Centrex.Infrastructure.MultiTenancy.Middleware;
using Saylo.Centrex.Infrastructure.MultiTenancy.Middleware.TenantResolvers;

namespace Saylo.Centrex.Infrastructure.MultiTenancy.Configuration;

public class MultiTenantBuilder
{
    private readonly IServiceCollection _services;
    private readonly IConfiguration _configuration;

    public MultiTenantBuilder(IServiceCollection services, IConfiguration configuration)
    {
        _services = services;
        _configuration = configuration;
    }

    public MultiTenantBuilder AddTenantResolvers(params Type[] resolvers)
    {
        foreach (var resolver in resolvers)
        {
            _services.AddTransient(typeof(ITenantResolver), resolver);
        }
        _services.AddTransient<ITenantResolver, TenantResolverPipeline>();
        return this;
    }

    public MultiTenantBuilder AddMultiTenantDbContext<TContext>(string? migrationsAssembly)
        where TContext : DbContext
    {
        var multiTenantConfig = _configuration.GetSection(nameof(MultiTenantConfig)).Get<MultiTenantConfig>();
        ArgumentNullException.ThrowIfNull(multiTenantConfig);

        _services.AddSingleton(multiTenantConfig);
        _services.AddHttpContextAccessor();
        _services.AddScoped<ITenantContextAccessor, TenantContextAccessor>();
        _services.AddDbContext<TContext>(options =>
        {
            options.UseSqlServer(multiTenantConfig.DefaultConnectionString, sql =>
            {
                if (!string.IsNullOrWhiteSpace(migrationsAssembly))
                {
                    sql.MigrationsAssembly(migrationsAssembly);
                }
            });
        });
        if (multiTenantConfig.ApplyMigrationsOnStartup)
        {
            _services.AddHostedService<MigrateDatabaseHostedService<TContext>>();
        }

        return this;
    }
    
    public MultiTenantBuilder AddDefaultTenant(Guid? tenantId, string connectionString)
    {
        _services.Configure<MultiTenantConfig>(config => { config.DefaultConnectionString = connectionString; });

        _services.AddSingleton<ITenantContextAccessor>(_ =>
            new TenantContextAccessor { TenantId = tenantId });

        return this;
    }

    public void Build()
    {
        // Any finalization steps can be added here.
    }
}
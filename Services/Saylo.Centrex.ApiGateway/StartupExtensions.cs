using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Saylo.Centrex.Application.Multitenancy;
using Saylo.Centrex.Infrastructure.Cache;
using Saylo.Centrex.Infrastructure.Identity;
using Saylo.Centrex.Infrastructure.MultiTenancy.Configuration;
using Saylo.Centrex.Infrastructure.MultiTenancy.Middleware.TenantResolvers;

namespace Saylo.Centrex.ApiGateway;

public static class StartupExtensions
{
    public static IServiceCollection AddGatewayConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddRedisCache(configuration);
        services.AddUserContext();
        var multiTenantConfig = configuration.GetSection(nameof(MultiTenantConfig)).Get<MultiTenantConfig>();
        ArgumentNullException.ThrowIfNull(multiTenantConfig);
        services.AddSingleton(multiTenantConfig);
        services.AddLogging(logging =>
        {
            logging.AddConsole()
                .AddDebug()
                .SetMinimumLevel(LogLevel.Debug);
        });
        services.AddTenantResolvers(typeof(HeaderTenantResolver), typeof(ParamQueryTenantResolver));
        services
            .AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"));
        services.AddServiceDiscovery();
        return services;
    }
    private static void AddTenantResolvers(this IServiceCollection serviceCollection,params Type[] resolvers)
    {
        foreach (var resolver in resolvers)
        {
            serviceCollection.AddTransient(typeof(ITenantResolver), resolver);
        }
        serviceCollection.AddTransient<ITenantResolver, TenantResolverPipeline>();
    }

}

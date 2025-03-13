using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Saylo.Centrex.Application.Common.ManualMapping;
using Saylo.Centrex.Domain.Specifications;
using Saylo.Centrex.Identity.Core.Application.Consumers;
using Saylo.Centrex.Identity.Core.Application.Interfaces;
using Saylo.Centrex.Identity.Core.Application.Models;
using Saylo.Centrex.Identity.Core.Application.Services;
using Saylo.Centrex.Identity.Core.Domain.Entities;
using Saylo.Centrex.Identity.Core.Domain.Entities.Identity;
using Saylo.Centrex.Identity.Core.Infrastructure.Configuration;
using Saylo.Centrex.Identity.Core.Infrastructure.OpenId;
using Saylo.Centrex.Identity.Core.Infrastructure.Persistence;
using Saylo.Centrex.Identity.Core.Infrastructure.Services;
using Saylo.Centrex.Infrastructure;
using Saylo.Centrex.Infrastructure.Cache;
using Saylo.Centrex.Infrastructure.Identity;
using Saylo.Centrex.Infrastructure.Mailing;
using Saylo.Centrex.Infrastructure.Messaging;
using Saylo.Centrex.Infrastructure.MultiTenancy.Configuration.Extensions;
using Saylo.Centrex.Infrastructure.Persistence;
using Saylo.Centrex.Infrastructure.Workers.Hangfire;

namespace Saylo.Centrex.Identity.Core;

public static class Startup
{
    public static IServiceCollection AddIdentityCore(this IServiceCollection services, IConfiguration configuration)
    {
        var assemblyName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

        // Core infrastructure setup
        ConfigureInfrastructure(services, configuration, assemblyName);

        // Identity configuration
        ConfigureIdentity(services, configuration);

        // Service registration
        RegisterServices(services, configuration);

        // External services configuration
        ConfigureExternalServices(services, configuration);

        return services;
    }

    private static void ConfigureInfrastructure(IServiceCollection services, IConfiguration configuration, string assemblyName)
    {
        services.AddInfrastructure<SayloIdentityDbContext>();
        services.ConfigureMultiTenancy<SayloIdentityDbContext>(
            configuration,
            assemblyName
        );
        services
            .AddRepositories<SayloIdentityDbContext>()
            .AddReadRepositories<SayloIdentityDbContext>();
        services.AddScoped<IProjectionProvider, ProjectionProvider>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<SayloIdentityDbContext>());

    }
    private static void ConfigureIdentity(IServiceCollection services, IConfiguration configuration)
    {
        var identitySettings = configuration
            .GetSection(nameof(IdentitySettings))
            .Get<IdentitySettings>();
        ArgumentNullException.ThrowIfNull(identitySettings);

        services
            .AddIdentity<ApplicationUser, AdminRole>(options =>
        {
            options.Password.RequiredLength = identitySettings.PasswordRequiredLength;
            options.Password.RequireDigit = identitySettings.PasswordIsRequireDigit;
            options.Password.RequireLowercase = identitySettings.PasswordIsRequireLowercase;
            options.Password.RequireNonAlphanumeric = identitySettings.PasswordIsRequireNonAlphanumeric;
            options.Password.RequireUppercase = identitySettings.PasswordIsRequireUppercase;
            options.Lockout.MaxFailedAccessAttempts = identitySettings.PasswordMaxFailedAccessAttempts;
        })
            .AddEntityFrameworkStores<SayloIdentityDbContext>()
            .AddDefaultTokenProviders();
    }
    private static void RegisterServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<UserManager<ApplicationUser>>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IPasswordHasher<ApplicationUser>, PasswordHasher<ApplicationUser>>();
        services.AddScoped<SeedDummyDataService>();
        services.AddUserContext();
    }
    private static void ConfigureExternalServices(IServiceCollection services, IConfiguration configuration)
    {
        // Token client configuration
        ConfigureTokenClient(services, configuration);

        // Message queue configuration
        services.AddRabbitMq(configuration, builder =>
        {
            builder.AddMessagePublishers()
                .AddConsumer<TenantConsumer>()
                .AddTenantFilter()
                .Build();
        });

        // Additional services
        services.AddHangfireServices(configuration);
        services.AddOpenIdServerConfig(configuration);
        services.AddRedisCache(configuration);
        services.AddEmailService(configuration);

    }
    private static void ConfigureTokenClient(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TokenClientOptions>(configuration.GetSection("TokenClient"));

        services.AddHttpClient("TokenClient", (serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<TokenClientOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        });
    }
}
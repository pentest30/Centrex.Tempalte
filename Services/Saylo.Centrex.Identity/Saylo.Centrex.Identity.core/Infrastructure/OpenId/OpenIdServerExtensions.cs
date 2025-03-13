using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Server;
using Saylo.Centrex.Identity.Core.Application.Interfaces;
using Saylo.Centrex.Identity.Core.Infrastructure.OpenId.Models;
using Saylo.Centrex.Identity.Core.Infrastructure.Persistence;
using Saylo.Centrex.Identity.Core.Infrastructure.Services;

namespace Saylo.Centrex.Identity.Core.Infrastructure.OpenId;

public static class OpenIdServerExtensions
{
    public static void AddOpenIdServerConfig(this IServiceCollection services, IConfiguration configuration)
    {
        var certificateSettings = ValidateAndGetCertificateSettings(configuration);
        RegisterServices(services, certificateSettings);
        var cert = InitializeKeyRotation(services);
        ConfigureOpenIddict(services, cert);
    }

    private static OpenIdCertificateSettings ValidateAndGetCertificateSettings(IConfiguration configuration)
    {
        var settings = configuration.GetSection("OpenIdCertificateSettings").Get<OpenIdCertificateSettings>();
        if (settings == null)
        {
            throw new InvalidOperationException("OpenIdCertificateSettings section is missing in configuration");
        }
        settings.Validate();
        return settings;
    }

    private static void RegisterServices(IServiceCollection services, OpenIdCertificateSettings certificateSettings)
    {
        // Application services
        services.AddScoped<IOpenIdAuthorizationService, OpenIdAuthorizationService>();
        services.AddScoped<ITokenClient, TokenHttpClient>();
        services.AddScoped<ICertificateMetadataRepository, EfCertificateMetadataRepository>();

        // Certificate management
        services.AddSingleton(certificateSettings);
        services.AddSingleton<ICertificateGenerator, CertificateGenerator>();
        services.AddSingleton<ICertificateStorage, FileSystemCertificateStorage>();

        // Key rotation setup
        services.AddSingleton<KeyRotationService>();
        services.AddHostedService(sp => sp.GetRequiredService<KeyRotationService>());
        services.AddScoped<IKeyRotationService>(sp => sp.GetRequiredService<KeyRotationService>());

        // OpenIddict configuration
        services.AddSingleton<IConfigureOptions<OpenIddictServerOptions>, OpenIddictServerConfiguration>();
    }

    private static X509Certificate2 InitializeKeyRotation(IServiceCollection services)
    {
        var sp = services.BuildServiceProvider();
        var keyRotationService = sp.GetRequiredService<KeyRotationService>();
        return keyRotationService.GetCurrentCertificateAsync().GetAwaiter().GetResult().Certificate;
    }

    private static void ConfigureOpenIddict(IServiceCollection services, X509Certificate2 certificate)
    {
        services.AddOpenIddict()
            .AddCoreConfiguration()
            .AddServerConfiguration(certificate)
            .AddValidationConfiguration();
    }

    private static OpenIddictBuilder AddCoreConfiguration(this OpenIddictBuilder builder)
    {
        return builder.AddCore(options =>
        {
            options.UseEntityFrameworkCore()
                .UseDbContext<SayloIdentityDbContext>();
        });
    }

    private static OpenIddictBuilder AddServerConfiguration(this OpenIddictBuilder builder, X509Certificate2 certificate)
    {
        return builder.AddServer(options =>
        {
            ConfigureEndpoints(options);
            ConfigureTokenValidation(options);
            ConfigureCertificates(options, certificate);
            ConfigureFlows(options);
            ConfigureScopes(options);
            ConfigureClaims(options);
            ConfigureAspNetCore(options);
        });
    }

    private static OpenIddictBuilder AddValidationConfiguration(this OpenIddictBuilder builder)
    {
        return builder.AddValidation(options =>
        {
            options.UseLocalServer();
            options.UseAspNetCore();
        });
    }

    private static void ConfigureEndpoints(OpenIddictServerBuilder options)
    {
        options
            .SetTokenEndpointUris("/connect/token")
            .SetUserInfoEndpointUris("/connect/userinfo")
            .SetIntrospectionEndpointUris("/connect/introspect")
            .SetAuthorizationEndpointUris("/connect/authorize");
    }

    private static void ConfigureTokenValidation(OpenIddictServerBuilder options)
    {
        options.SetAccessTokenLifetime(TimeSpan.FromDays(5));
        options.Configure(options =>
        {
            options.TokenValidationParameters.ValidateAudience = true;
            options.TokenValidationParameters.ValidateLifetime = true;
            options.TokenValidationParameters.ValidateIssuerSigningKey = true;
            options.TokenValidationParameters.ValidAudiences = new[] { "api", "api-client" };
            options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(5);
        });
    }

    private static void ConfigureCertificates(OpenIddictServerBuilder options, X509Certificate2 certificate)
    {
        options.DisableAccessTokenEncryption();
        options.AddEncryptionCertificate(certificate)
            .AddSigningCertificate(certificate);
    }

    private static void ConfigureFlows(OpenIddictServerBuilder options)
    {
        options
            .AllowPasswordFlow()
            .AllowRefreshTokenFlow()
            .AllowClientCredentialsFlow()
            .AllowAuthorizationCodeFlow();
    }

    private static void ConfigureScopes(OpenIddictServerBuilder options)
    {
        options.RegisterScopes(
            "api",
            "email",
            "profile",
            "roles",
            "tenantId"
        );
    }

    private static void ConfigureClaims(OpenIddictServerBuilder options)
    {
        options.RegisterClaims(
            OpenIddictConstants.Claims.Subject,
            OpenIddictConstants.Claims.Name,
            OpenIddictConstants.Claims.Email,
            OpenIddictConstants.Claims.Role,
            "tenantId"
        );
    }
    private static void ConfigureAspNetCore(OpenIddictServerBuilder options)
    {
        options
            .UseAspNetCore()
            .DisableTransportSecurityRequirement()
            .EnableTokenEndpointPassthrough()
            .EnableUserInfoEndpointPassthrough();
    }
    
}
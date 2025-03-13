using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using OpenIddict.Validation.AspNetCore;
using Saylo.Centrex.Application.Common.Identity;

namespace Saylo.Centrex.Infrastructure.Identity;

public static class CurrentUserServiceExtensions
{
    public static IServiceCollection AddUserContext(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        return services;
    }

    public static IServiceCollection AddOpenIdAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var config  = configuration.GetSection("Authentication").Get<AuthenticationConfig>();
        ArgumentNullException.ThrowIfNull(config);
        
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            options.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            options.DefaultForbidScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
        });

        services.AddOpenIddict()
            .AddValidation(options =>
            {
                options.UseIntrospection()
                    .SetIssuer(config.Authority)
                    .SetClientId(config.ClientId)          
                    .SetClientSecret(config.ClientSecret);

                options.UseAspNetCore();
                options.UseSystemNetHttp();

                options.Configure(options =>
                {
                    options.TokenValidationParameters.ValidateAudience = false;
                    options.TokenValidationParameters.ValidAudiences = new[] { "api" };
                    options.TokenValidationParameters.ValidateIssuer = true;
                    options.TokenValidationParameters.ValidIssuer = configuration["Authentication:Authority"];
                    options.TokenValidationParameters.ValidateLifetime = true;
                    options.TokenValidationParameters.NameClaimType = OpenIddictConstants.Claims.Name;
                    options.TokenValidationParameters.RoleClaimType = OpenIddictConstants.Claims.Role;
                });
            });

        return services;
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Saylo.Centrex.Infrastructure.MultiTenancy.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Saylo.Centrex.Infrastructure.Swagger;

public static class SwaggerExtensions
{
    public static void AddSwaggerConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var oauth2Settings = new SwaggerConfig();
        configuration.GetSection(nameof(SwaggerConfig)).Bind(oauth2Settings);

        var authServerUrl = oauth2Settings.AuthServerUrl ?? "https://localhost:7071";

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(oauth2Settings.ApiVersion, new OpenApiInfo
            {
                Title = oauth2Settings.ApiDescription,
                Version = oauth2Settings.ApiVersion
            });
            
            options.MapType<JsonResult>(() => new OpenApiSchema { Type = "object" });

            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.OAuth2,
                Scheme = "oauth2",
                In = ParameterLocation.Header,
                Flows = new OpenApiOAuthFlows
                {
                    Password = new OpenApiOAuthFlow
                    {
                        TokenUrl = new Uri($"{authServerUrl}/connect/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "api", "API access" },
                            { "email", "Email information" },
                            { "profile", "Profile information" },
                            { "roles", "User roles" }
                        }
                    }
                }
            });

            // Add operation filters
            options.OperationFilter<SecurityRequirementsOperationFilter>();
            options.OperationFilter<TenantHeaderOperationFilter>();
        });
    }

    public static void UseSwaggerConfiguration(this IApplicationBuilder app, IConfiguration configuration)
    {
        var oauth2Settings = new SwaggerConfig();
        configuration.GetSection(nameof(SwaggerConfig)).Bind(oauth2Settings);

        app.UseSwagger(options =>
        {
            options.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
            {
                swaggerDoc.Servers = new List<OpenApiServer>
                {
                    new() { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" }
                };
            });
        });

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint($"/swagger/{oauth2Settings.ApiVersion}/swagger.json",
                oauth2Settings.ApiDescription);

            options.OAuthConfigObject = new OAuthConfigObject
            {
                AppName = oauth2Settings.ApiDescription,
                ClientId = oauth2Settings.ClientId,
                ClientSecret = oauth2Settings.ClientSecret,
                Scopes = new[] { "api", "email", "profile", "roles" },
                AdditionalQueryStringParams = new Dictionary<string, string>
                {
                    { "resource", oauth2Settings.ClientId }
                },
                UsePkceWithAuthorizationCodeGrant = false
            };

            options.OAuth2RedirectUrl($"{oauth2Settings.AuthServerUrl}/swagger/oauth2-redirect.html");
            options.OAuthClientId(oauth2Settings.ClientId);
            options.OAuthClientSecret(oauth2Settings.ClientSecret);
            options.OAuthRealm(oauth2Settings.ClientId);
            options.OAuthAppName(oauth2Settings.ApiDescription);
            
            options.DisplayRequestDuration();
            options.EnableDeepLinking();
            options.ShowExtensions();
            options.EnableValidator();
        });
    }
}
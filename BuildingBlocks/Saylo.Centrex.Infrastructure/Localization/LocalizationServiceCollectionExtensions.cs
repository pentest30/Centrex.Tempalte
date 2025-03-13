using System.Globalization;
using FullMvnoSolution.Infrastructure.Localization.PoFileConfig;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Saylo.Centrex.Infrastructure.Localization.JsonConfig;
using Saylo.Centrex.Infrastructure.Localization.PoFileConfig;

namespace Saylo.Centrex.Infrastructure.Localization
{
    public static class LocalizationServiceCollectionExtensions
    {
        public static IServiceCollection AddLocalization(this IServiceCollection services, IConfiguration config)
        {
            
            var providers = config.GetSection(nameof(LocalizationProviders)).Get<LocalizationProviders>();

            if (providers?.JsonFile is { IsEnabled: true })
            {
                services.Configure<JsonOptions>(op =>
                {
                    op.ResourcesPath = providers.JsonFile.ResourcesPath;
                });
                services.AddSingleton(providers.JsonFile);
                services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
            }
            else if (providers?.PoFile is { IsEnabled: true })
            {
                services.Configure<PoFileOptions>(op =>
                {
                    op.ResourcesPath = providers.PoFile.ResourcesPath;
                });
                services.AddSingleton(providers.PoFile);
                services.AddSingleton<IStringLocalizerFactory, PoStringLocalizerFactory>();
            }

            AddCultureConfig(services, providers);
            return services;
        }

        private static void AddCultureConfig(IServiceCollection services,LocalizationProviders? providers)
        {
            services.AddScoped<IStringLocalizer>(provider => provider.GetRequiredService<IStringLocalizerFactory>().Create(null));
            services.AddLocalization();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(providers?.DefaultRequestCulture ?? "fr-FR");
                if (providers?.SupportedCultures != null)
                {
                    var supportedCultures = providers.SupportedCultures.Select(x => new CultureInfo(x)).ToList();
                    options.SupportedCultures = supportedCultures;
                    options.SupportedUICultures = supportedCultures;
                }
            });
        }
    }
}

using System.Text;
using FullMvnoSolution.Infrastructure.Localization.PoFileConfig;
using Karambolo.PO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace Saylo.Centrex.Infrastructure.Localization.PoFileConfig
{
    public class PoStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;
        private readonly RequestLocalizationOptions _requestLocalizationOptions;
        private readonly PoFileOptions _poFileOptions;

        public PoStringLocalizerFactory(IHttpContextAccessor httpContextAccessor,
            IMemoryCache memoryCache,
            IOptions<RequestLocalizationOptions> requestLocalizationOptions,
            PoFileOptions poFileOptions)
        {
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
            _requestLocalizationOptions = requestLocalizationOptions.Value;
            _poFileOptions = poFileOptions;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new PoStringLocalizer(_httpContextAccessor, _requestLocalizationOptions, LoadData());
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new PoStringLocalizer(_httpContextAccessor, _requestLocalizationOptions, LoadData());
        }

        private Dictionary<string, Dictionary<string, string>> LoadData()
        {
            var data = _memoryCache.Get<Dictionary<string, Dictionary<string, string>>>(typeof(PoStringLocalizerFactory).FullName);

            if (data == null)
            {
                data = this.LoadTranslations(_poFileOptions.ResourcesPath);
                _memoryCache.Set(typeof(PoStringLocalizerFactory).FullName, data);
            }

            return data;
        }

        private Dictionary<string, Dictionary<string, string>> LoadTranslations(string resourcesPath)
        {
            var translationData = new Dictionary<string, Dictionary<string, string>>();
            var translationDirectories = Directory.GetDirectories(resourcesPath);

            foreach (var directory in translationDirectories)
            {
                var languageCode = Path.GetFileName(directory);
                var translations = this.LoadTranslationsForLanguage(directory);
                if (translations.Count > 0)
                {
                    translationData[languageCode] = translations;
                }
            }

            return translationData;
        }

        private Dictionary<string, string> LoadTranslationsForLanguage(string directory)
        {
            var translations = new Dictionary<string, string>();
            var poFiles = Directory.GetFiles(directory, "*.po");

            foreach (var file in poFiles)
            {
                var fileTranslations = this.ParseTranslationFile(file);
                foreach (var translation in fileTranslations)
                {
                    translations[translation.Key] = translation.Value;
                }
            }

            return translations;
        }

        private Dictionary<string, string> ParseTranslationFile(string file)
        {
            var translations = new Dictionary<string, string>();

            using var reader = new StreamReader(file, Encoding.UTF8);
            var parser = new POParser();
            var poParseResult = parser.Parse(reader);

            if (!poParseResult.Success)
            {
                return translations;
            }

            foreach (var key in poParseResult.Catalog.Keys)
            {
                var translation = poParseResult.Catalog.GetTranslation(key);
                if (!string.IsNullOrEmpty(translation) && !string.IsNullOrEmpty(key.Id))
                {
                    translations[key.Id] = translation;
                }
            }

            return translations;
        }

    }
}

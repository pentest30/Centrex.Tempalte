using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Saylo.Centrex.Infrastructure.Localization.JsonConfig
{
    public class JsonStringLocalizerFactory : IStringLocalizerFactory
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;
        private readonly JsonOptions _options;
        private readonly RequestLocalizationOptions _requestLocalizationOptions;

        public JsonStringLocalizerFactory(IHttpContextAccessor httpContextAccessor,
            IMemoryCache memoryCache, 
            IOptions<RequestLocalizationOptions> requestLocalizationOptions, 
            JsonOptions options)
        {
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
            _options = options;
            _requestLocalizationOptions = requestLocalizationOptions.Value;
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            return new JsonStringLocalizer(_httpContextAccessor, _requestLocalizationOptions, LoadData());
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return new JsonStringLocalizer(_httpContextAccessor, _requestLocalizationOptions, LoadData());
        }

        private Dictionary<string, Dictionary<string, string>> LoadData()
        {
            var data = _memoryCache.Get<Dictionary<string, Dictionary<string, string>>>(typeof(JsonStringLocalizerFactory).FullName);

            if (data == null)
            {
                data = new Dictionary<string, Dictionary<string, string>>();
                var translationFiles = Directory.GetFiles(_options.ResourcesPath, "*.json");

                foreach (var file in translationFiles)
                {
                    var languageCode = Path.GetFileNameWithoutExtension(file);
                    var jsonContent = File.ReadAllText(file);

                    var translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonContent);

                    if (translations != null) data.Add(languageCode, translations);
                }
                _memoryCache.Set(typeof(JsonStringLocalizerFactory).FullName, data);
            }

            return data;
        }
    }
}

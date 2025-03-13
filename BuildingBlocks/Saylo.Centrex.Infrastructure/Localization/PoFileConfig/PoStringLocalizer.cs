using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;

namespace Saylo.Centrex.Infrastructure.Localization.PoFileConfig
{
    public class PoStringLocalizer : IStringLocalizer
    {
        private readonly Dictionary<string, Dictionary<string, string>> _translations;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RequestLocalizationOptions _requestLocalizationOptions;

        public PoStringLocalizer(IHttpContextAccessor httpContextAccessor,
            RequestLocalizationOptions requestLocalizationOptions,
            Dictionary<string, Dictionary<string, string>> translations)
        {
            _httpContextAccessor = httpContextAccessor;
            _requestLocalizationOptions = requestLocalizationOptions;
            _translations = translations;
        }

        public LocalizedString this[string name]
        {
            get
            {
                var value = GetTranslation(name);
                return new LocalizedString(name, value);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var format = GetTranslation(name);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var languageCode = GetLanguageCode();
            if (!_translations.TryGetValue(languageCode, out var languageTranslations)) yield break;
            foreach (var keyValuePair in languageTranslations)
            {
                yield return new LocalizedString(keyValuePair.Key, keyValuePair.Value);
            }
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            // TODO: Consider implementing culture change within a single request context if needed.
            throw new NotImplementedException();
        }

        private string GetTranslation(string key)
        {
            var languageCode = GetLanguageCode();
            if (!_translations.TryGetValue(languageCode, out var languageTranslations)) return key;
            return languageTranslations.GetValueOrDefault(key, key);
        }

        private string GetLanguageCode()
        {
            if (_requestLocalizationOptions is null || _httpContextAccessor is null) return "fr-FR";
            var defaultCulture = _requestLocalizationOptions.DefaultRequestCulture.Culture.ToString();
            var languageCode = _httpContextAccessor.HttpContext?.Request.GetTypedHeaders().AcceptLanguage.FirstOrDefault()?.Value.ToString() ??
                defaultCulture;
            return languageCode;
        }
    }
}

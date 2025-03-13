using FullMvnoSolution.Infrastructure.Localization.PoFileConfig;
using Saylo.Centrex.Infrastructure.Localization.JsonConfig;

namespace Saylo.Centrex.Infrastructure.Localization
{
    public class LocalizationProviders
    {
        public LocalizationProviders()
        {
        }
        public JsonOptions? JsonFile { get; set; }
        public PoFileOptions? PoFile { get; set; }
        
        public string[]? SupportedCultures { get; set; }
       
        public string? DefaultRequestCulture { get; set; }
    }
}

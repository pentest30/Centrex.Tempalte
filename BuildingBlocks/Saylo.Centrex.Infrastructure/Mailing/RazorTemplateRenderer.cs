using RazorLight;
using Saylo.Centrex.Application.Mailing;

namespace Saylo.Centrex.Infrastructure.Mailing;

public class RazorTemplateRenderer : ITemplateRenderer
{
    private readonly string _templateDirectory;
    private readonly IRazorLightEngine _engine;

    public RazorTemplateRenderer(string templateDirectory)
    {
        _templateDirectory = templateDirectory;
        _engine = new RazorLightEngineBuilder()
            .UseFileSystemProject(_templateDirectory)
            .UseMemoryCachingProvider()
            .Build();
    }

    public async Task<string> RenderTemplateAsync(string templateName, object model)
    {
        var templatePath = Path.Combine(_templateDirectory, $"{templateName}.cshtml");
        return await _engine.CompileRenderAsync(templatePath, model);
    }
}
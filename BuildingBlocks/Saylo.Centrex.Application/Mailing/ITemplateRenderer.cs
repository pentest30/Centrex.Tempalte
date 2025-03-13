namespace Saylo.Centrex.Application.Mailing;

public interface ITemplateRenderer
{
    Task<string> RenderTemplateAsync(string templateName, object model);
}
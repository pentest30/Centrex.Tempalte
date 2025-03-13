namespace Saylo.Centrex.Application.Mailing;

public class EmailMessage
{
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public string TemplateName { get; set; }
    public object TemplateData { get; set; }
}
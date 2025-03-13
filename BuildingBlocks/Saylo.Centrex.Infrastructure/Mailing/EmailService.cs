using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using Saylo.Centrex.Application.Mailing;

namespace Saylo.Centrex.Infrastructure.Mailing;

public class EmailService : IEmailService
{
    private readonly SmtpConfiguration _config;
    private readonly ITemplateRenderer _templateRenderer;

    public EmailService(IOptions<SmtpConfiguration> config, ITemplateRenderer templateRenderer)
    {
        _config = config.Value;
        _templateRenderer = templateRenderer;
    }

    public async Task SendEmailAsync(EmailMessage message)
    {
        using var client = new SmtpClient(_config.Host, _config.Port)
        {
            EnableSsl = _config.EnableSsl,
            Credentials = new NetworkCredential(_config.Username, _config.Password)
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_config.FromEmail),
            Subject = message.Subject,
            Body = message.Body,
            IsBodyHtml = true,
            To = { message.To }
        };

        await client.SendMailAsync(mailMessage);
    }

    public async Task SendTemplatedEmailAsync(EmailMessage message)
    {
        var renderedBody = await _templateRenderer.RenderTemplateAsync(
            message.TemplateName, 
            message.TemplateData
        );
        
        message.Body = renderedBody;
        await SendEmailAsync(message);
    }
}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Saylo.Centrex.Application.Mailing;

namespace Saylo.Centrex.Infrastructure.Mailing;

public static class MailingExtensions
{
    public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<SmtpConfiguration>(config.GetSection("EmailSettings:SmtpConfig"));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<SmtpConfiguration>>().Value);
        services.AddTransient<IEmailService, EmailService>();
        services.AddSingleton<ITemplateRenderer>(sp => 
            new RazorTemplateRenderer(Path.Combine(Directory.GetCurrentDirectory(), "Templates\\Email")));
        return services;
    }
}
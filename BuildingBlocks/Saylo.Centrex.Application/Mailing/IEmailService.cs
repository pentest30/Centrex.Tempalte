namespace Saylo.Centrex.Application.Mailing;

public interface IEmailService
{
    Task SendEmailAsync(EmailMessage message);
    Task SendTemplatedEmailAsync(EmailMessage message);
}
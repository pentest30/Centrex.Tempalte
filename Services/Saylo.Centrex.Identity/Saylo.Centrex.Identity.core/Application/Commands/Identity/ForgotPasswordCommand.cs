using MediatR;
using Microsoft.AspNetCore.Identity;
using Saylo.Centrex.Application.Common.Commands;
using Saylo.Centrex.Application.Mailing;
using Saylo.Centrex.Identity.Core.Application.Interfaces;
using Saylo.Centrex.Identity.Core.Application.Models;
using Saylo.Centrex.Identity.Core.Domain.Entities.Identity;

namespace Saylo.Centrex.Identity.Core.Application.Commands.Identity;

public class ForgotPasswordCommand : BaseRequest<IdentityResponse>
{
    public string Email { get; set; }
}
public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, IdentityResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IEmailService _emailService;
    private readonly UserManager<ApplicationUser> _userManager;

    public ForgotPasswordCommandHandler(
        IIdentityService identityService,
        IEmailService emailService,
        UserManager<ApplicationUser> userManager)
    {
        _identityService = identityService;
        _emailService = emailService;
        _userManager = userManager;
    }

    public async Task<IdentityResponse> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetUserByEmailAsync(command.Email);
        if (user == null)
        {
            return IdentityResponse.Success("If your email is registered, you will receive reset instructions");
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var message = BuildEmailMessage(token, user);

        await _emailService.SendTemplatedEmailAsync(message);
        return IdentityResponse.Success("If your email is registered, you will receive reset instructions.");
    }

    private static EmailMessage BuildEmailMessage(string token, ApplicationUser user)
    {
        var resetLink = $"https://localhost:7071/reset-password?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";
        var emailModel = new ResetPasswordEmailModel
        {
            UserName = user.UserName,
            ResetPasswordUrl = resetLink,
            CompanyName = "Test Company"
        };

        var message = new EmailMessage
        {
            To = user.Email,
            Subject = "Password Reset",
            TemplateName = "ResetPasswordEmail",
            TemplateData =  emailModel
        };
        return message;
    }
}
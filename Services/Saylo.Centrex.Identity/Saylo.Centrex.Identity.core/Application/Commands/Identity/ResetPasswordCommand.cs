using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Saylo.Centrex.Application.Common.Commands;
using Saylo.Centrex.Identity.Core.Application.Interfaces;
using Saylo.Centrex.Identity.Core.Application.Models;
using Saylo.Centrex.Identity.Core.Domain.Entities.Identity;
using Saylo.Centrex.Identity.Core.Infrastructure.Configuration;

namespace Saylo.Centrex.Identity.Core.Application.Commands.Identity;

public class ResetPasswordCommand : BaseRequest<IdentityResponse>
{
    public string Token { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmPassword { get; set; }
    public string Email { get; set; }
}
public class ResetPasswordValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordValidator(AppSettings appSettings)
    {
        RuleFor(user => user.NewPassword)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(appSettings.IdentitySettings.PasswordRequiredLength)
            .WithMessage("Password must be at least 8 characters long.");

        RuleFor(user => user.NewPassword)
            .Matches("[A-Z]")
            .WithMessage("Password must contain at least one uppercase letter.")
            .When(user => appSettings.IdentitySettings.PasswordIsRequireUppercase, ApplyConditionTo.CurrentValidator);

        RuleFor(user => user.NewPassword)
            .Matches("[a-z]")
            .WithMessage("Password must contain at least one lowercase letter.")
            .When(user => appSettings.IdentitySettings.PasswordIsRequireLowercase, ApplyConditionTo.CurrentValidator);

        RuleFor(user => user.NewPassword)
            .Matches("[0-9]")
            .WithMessage("Password must contain at least one digit.")
            .When(user => appSettings.IdentitySettings.PasswordIsRequireDigit, ApplyConditionTo.CurrentValidator);

        RuleFor(user => user.NewPassword)
            .Matches("[@#$%^&*(),.?\":{}|<>]")
            .WithMessage("Password must contain at least one special character.")
            .When(user => appSettings.IdentitySettings.PasswordIsRequireNonAlphanumeric, ApplyConditionTo.CurrentValidator);


        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Token)
            .NotEmpty();
        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.NewPassword).WithMessage("Passwords do not match");
    }
}
public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand, IdentityResponse>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IIdentityService _identityService;
    private readonly ILogger<ResetPasswordHandler> _logger;

    public ResetPasswordHandler(
        UserManager<ApplicationUser> userManager,
        IIdentityService identityService,
        ILogger<ResetPasswordHandler> logger)
    {
        _userManager = userManager;
        _identityService = identityService;
        _logger = logger;
    }

    public async Task<IdentityResponse> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var user = await _identityService.GetUserByEmailAsync(command.Email);
            if (user == null)
            {
                return IdentityResponse.Failure("Invalid reset attempt");
            }

            if (!await ValidateTokenAsync(user, command.Token))
            {
                return IdentityResponse.Failure("Invalid or expired token");
            }

            var result = await _userManager.ResetPasswordAsync(user, command.Token, command.NewPassword);
            if (result.Succeeded)
            {
                _logger.LogInformation("Password reset successful for user {Email}", command.Email);
                return IdentityResponse.Success("Password has been reset successfully");
            }

            return IdentityResponse.Failure(result.Errors.Select(e => e.Description));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for user {Email}", command.Email);
            return IdentityResponse.Failure("An error occurred while resetting the password");
        }
    }

    private async Task<bool> ValidateTokenAsync(ApplicationUser user, string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var purpose = _userManager.Options.Tokens.PasswordResetTokenProvider;
            return await _userManager.VerifyUserTokenAsync(user, purpose, "ResetPassword", token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating reset token for user {UserId}", user.Id);
            return false;
        }
    }
}
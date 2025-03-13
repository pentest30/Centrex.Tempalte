using FluentValidation;
using Saylo.Centrex.Identity.Core.Infrastructure.Configuration;

namespace Saylo.Centrex.Identity.Core.Application.Commands.Identity;

public static class PasswordValidationRules
{
    public static IRuleBuilderOptions<T, string> ValidatePassword<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        AppSettings appSettings)
    {
        var builder = ruleBuilder
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(appSettings.IdentitySettings.PasswordRequiredLength)
            .WithMessage($"Password must be at least {appSettings.IdentitySettings.PasswordRequiredLength} characters long.");

        if (appSettings.IdentitySettings.PasswordIsRequireUppercase)
        {
            builder = builder
                .Matches("[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter.");
        }

        if (appSettings.IdentitySettings.PasswordIsRequireLowercase)
        {
            builder = builder
                .Matches("[a-z]")
                .WithMessage("Password must contain at least one lowercase letter.");
        }

        if (appSettings.IdentitySettings.PasswordIsRequireDigit)
        {
            builder = builder
                .Matches("[0-9]")
                .WithMessage("Password must contain at least one digit.");
        }

        if (appSettings.IdentitySettings.PasswordIsRequireNonAlphanumeric)
        {
            builder = builder
                .Matches("[@#$%^&*(),.?\":{}|<>]")
                .WithMessage("Password must contain at least one special character.");
        }

        return builder;
    }
}
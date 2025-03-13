using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Saylo.Centrex.Application.Common.Commands;
using Saylo.Centrex.Identity.Core.Application.Interfaces;
using Saylo.Centrex.Identity.Core.Application.Models;
using Saylo.Centrex.Identity.Core.Domain.Entities.Identity;
using Saylo.Centrex.Identity.Core.Infrastructure.Configuration;

namespace Saylo.Centrex.Identity.Core.Application.Commands.Identity;

public class CreateUserCommande : BaseRequest<IdentityResponse>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string UserName { get; set; }
    public TypeUser TypeUser { get; set; }
    public string ConfirmPassword { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

public class CreateUserCommandeValidator : AbstractValidator<CreateUserCommande>
{
    public CreateUserCommandeValidator(AppSettings appSettings, IIdentityService _userManager)
    {
        RuleFor(user => user.Password)
            .ValidatePassword(appSettings);
        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Email invalide")
            .Must(email =>  _userManager.GetUserByEmailAsync(email).GetAwaiter().GetResult() == null)
            .WithMessage("Email is already taken.");

        RuleFor(user => user.UserName)
            .NotEmpty()
            .WithMessage("UserName is required.")
            .Length(5, 20)
            .WithMessage("Username must be between 5 and 20 characters")
            .Must( Username =>  _userManager.GetUserByUserNameAsync(Username).Result == null)
            .WithMessage("Username is already taken.");

        RuleFor(user => user.TypeUser)
            .IsInEnum()
            .WithMessage("TypeUser is invalide.");

    }
}

public class CreateUserCommandeHandler(IIdentityService identityService)
    : IRequestHandler<CreateUserCommande, IdentityResponse>
{
    public async Task<IdentityResponse> Handle(CreateUserCommande request, CancellationToken cancellationToken)
    {
        var result = await identityService.RegisterAsync(request);
        return result;
    }
}
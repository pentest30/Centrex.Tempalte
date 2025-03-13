using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Saylo.Centrex.Application.Common.Commands;
using Saylo.Centrex.Application.Common.Identity;
using Saylo.Centrex.Identity.Core.Application.Interfaces;
using Saylo.Centrex.Identity.Core.Application.Models;
using Saylo.Centrex.Identity.Core.Domain.Entities.Identity;
using Saylo.Centrex.Identity.Core.Infrastructure.Configuration;

namespace Saylo.Centrex.Identity.Core.Application.Commands.Identity;

public  class AdminResetUserPasswordCommand : BaseRequest<IdentityResponse>
{
    public Guid UserId { get; set; }
    public string NewPassword { get; set; }
}
public class AdminResetUserPasswordCommandValidator : AbstractValidator<AdminResetUserPasswordCommand>
{
    public AdminResetUserPasswordCommandValidator(AppSettings appSettings)
    {
        RuleFor(user => user.NewPassword)
            .ValidatePassword(appSettings);
    }
}

public class AdminResetUserPasswordCommandHandler : IRequestHandler<AdminResetUserPasswordCommand, IdentityResponse>
{
    private readonly IIdentityService _identityService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminResetUserPasswordCommandHandler(
        IIdentityService identityService,
        UserManager<ApplicationUser> userManager)
    {
        _identityService = identityService;
        _userManager = userManager;
    }

    public async Task<IdentityResponse> Handle(AdminResetUserPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetUserByIdAsync(request.UserId.ToString());
        if (user == null) return IdentityResponse.Failure("User not found");

        // Generate reset token
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        
        // Reset password using the token
        var result = await _userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);
        
        return result.Succeeded 
            ? IdentityResponse.Success("Password has been reset successfully") 
            : IdentityResponse.Failure(result.Errors.Select(e => e.Description));
    }
}

// User Command to change their own password
public class ChangeOwnPasswordCommand : BaseRequest<IdentityResponse>
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}

public class ChangeOwnPasswordCommandValidator : AbstractValidator<ChangeOwnPasswordCommand>
{
    public ChangeOwnPasswordCommandValidator(AppSettings appSettings)
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(user => user.NewPassword)
            .ValidatePassword(appSettings);
    }
}

public class ChangeOwnPasswordCommandHandler : IRequestHandler<ChangeOwnPasswordCommand, IdentityResponse>
{
    private readonly IIdentityService _identityService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICurrentUserService _currentUserService;

    public ChangeOwnPasswordCommandHandler(
        IIdentityService identityService,
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUserService)
    {
        _identityService = identityService;
        _userManager = userManager;
        _currentUserService = currentUserService;
    }

    public async Task<IdentityResponse> Handle(ChangeOwnPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _identityService.GetUserByIdAsync(_currentUserService.UserId);
        if (user == null) return IdentityResponse.Failure("User not found");

        // Verify current password
        if (!await _userManager.CheckPasswordAsync(user, request.CurrentPassword))
            return IdentityResponse.Failure("Current password is incorrect");

        // Change password
        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

        return result.Succeeded 
            ? IdentityResponse.Success("Password has been changed successfully") 
            : IdentityResponse.Failure(result.Errors.Select(e => e.Description));
    }
}
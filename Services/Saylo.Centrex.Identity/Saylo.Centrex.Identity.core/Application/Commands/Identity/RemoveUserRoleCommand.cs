using FluentValidation;
using MediatR;
using Saylo.Centrex.Application.Common.Commands;
using Saylo.Centrex.Identity.Core.Application.Interfaces;

namespace Saylo.Centrex.Identity.Core.Application.Commands.Identity;

public class RemoveUserRoleCommand : BaseRequest<bool>
{
    public Guid UserId { get; set; }
    public string RoleName { get; set; }
}
public class RemoveUserRoleCommandValidator : AbstractValidator<CreateUserRoleCommand>
{
    public RemoveUserRoleCommandValidator()
    {
        RuleFor(user => user.RoleName)
            .NotEmpty()
            .WithMessage("Role name is required.");
    }
}
 public class RemoveUserRoleCommandHandler : IRequestHandler<RemoveUserRoleCommand, bool>
 {
     private readonly IIdentityService _identityService;

     public RemoveUserRoleCommandHandler(IIdentityService identityService)
     {
         _identityService = identityService;
     }
     public Task<bool> Handle(RemoveUserRoleCommand request, CancellationToken cancellationToken)
     {
         return _identityService.RemoveRoleAsync(request.UserId.ToString(), request.RoleName);
     }
 }
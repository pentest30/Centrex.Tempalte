using FluentValidation;
using MediatR;
using Saylo.Centrex.Application.Common.Commands;
using Saylo.Centrex.Identity.Core.Application.Interfaces;

namespace Saylo.Centrex.Identity.Core.Application.Commands.Identity;

public class CreateUserRoleCommand : BaseRequest<bool>
{
    public Guid UserId { get; set; }
    public string RoleName { get; set; }
}
public class CreateUserRoleCommandValidator : AbstractValidator<CreateUserRoleCommand>
{
    public CreateUserRoleCommandValidator()
    {
        RuleFor(user => user.RoleName).NotEmpty().WithMessage("Role name is required.");
    }
}   
public class CreateUserRoleCommandHandler(IIdentityService service) : IRequestHandler<CreateUserRoleCommand, bool>
{
    public Task<bool> Handle(CreateUserRoleCommand request, CancellationToken cancellationToken)
    {
        return service.AssignRoleAsync(request.UserId, request.RoleName);
    }
}
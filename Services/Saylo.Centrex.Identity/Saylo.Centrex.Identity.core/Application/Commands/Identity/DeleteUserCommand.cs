using Saylo.Centrex.Application.Common.Commands;
using Saylo.Centrex.Identity.Core.Application.Interfaces;
using MediatR;

namespace Saylo.Centrex.Identity.Core.Application.Commands.Identity;

// Command to delete a user
public class DeleteUserCommand : BaseRequest<bool>
{
    public Guid UserId { get; set; }
}

// Handler for the delete user command
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly IIdentityService _identityService;

    public DeleteUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
         return await _identityService.RemoveUserAsync(request.UserId.ToString());
    }
}
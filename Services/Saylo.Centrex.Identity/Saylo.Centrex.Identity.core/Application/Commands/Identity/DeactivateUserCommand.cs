using MediatR;
using Saylo.Centrex.Application.Common.Commands;
using Saylo.Centrex.Identity.Core.Application.Interfaces;

namespace Saylo.Centrex.Identity.Core.Application.Commands.Identity;

public class DeactivateUserCommand : BaseRequest<bool>
{
    public Guid UserId { get; set; }
}
public class DeactivateUserCommandHandler : IRequestHandler<DeactivateUserCommand, bool>
{
    private readonly IIdentityService _identityService;

    public DeactivateUserCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    public Task<bool> Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
       return _identityService.DeactivateUserAsync(request.UserId.ToString());
    }
}

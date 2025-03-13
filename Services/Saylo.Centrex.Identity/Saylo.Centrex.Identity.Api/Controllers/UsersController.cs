using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saylo.Centrex.Application.Common.Queries;
using Saylo.Centrex.Identity.Core.Application.Commands.Identity;
using Saylo.Centrex.Identity.Core.Application.DTOs;
using Saylo.Centrex.Identity.Core.Application.Models;
using Saylo.Centrex.Identity.Core.Application.Queries.Identity;
using Saylo.Centrex.Infrastructure.Identity;
using Saylo.Centrex.Infrastructure.Web.Apis;
using RegisterRequest = Saylo.Centrex.Identity.Core.Application.Models.RegisterRequest;

namespace Saylo.Centrex.Identity.Api.Controllers;

public class UsersController : ApiControllerBase
{

    public UsersController(
        IMediator dispatcher,
        IMapper mapper,
        ILogger<UsersController> logger) : base(dispatcher, mapper, logger)
    {
    }

    [Authorize(Roles = ApplicationRoles.Admin)]
    [HttpPost("create-user")]
    public async Task<ActionResult<IdentityResponse>> CreateUser([FromBody] RegisterRequest request)
    {
        var result = await SendCommandAsync<CreateUserCommande, RegisterRequest, IdentityResponse>(request);
        return Ok(result);
    }

    [Authorize(Roles = ApplicationRoles.Admin)]
    [HttpPut("{id:guid}/assign-user-role")]
    public async Task<ActionResult<bool>> CreateUserRole([FromRoute] Guid id, [FromBody] CreateUserRoleCommand request)
    {
        if (id != request.UserId) return BadRequest();
        var result = await SendCommandAsync<CreateUserRoleCommand, CreateUserRoleCommand, Boolean>(request);
        return Ok(result);
    }

    [Authorize(Roles = ApplicationRoles.Admin)]
    [HttpPut("{id:guid}/deactivate-user")]
    public async Task<ActionResult<bool>> DeactivateUser([FromRoute] Guid id, [FromBody] DeactivateUserCommand request)
    {
        if (id != request.UserId) return BadRequest();
        var result = await SendCommandAsync<DeactivateUserCommand, DeactivateUserCommand, Boolean>(request);
        return Ok(result);
    }

    [Authorize(Roles = ApplicationRoles.Admin)]
    [HttpPut("{id:guid}/remove-user-role")]
    public async Task<ActionResult<bool>> RemoveUserRole([FromRoute] Guid id, [FromBody] RemoveUserRoleCommand request)
    {
        if (id != request.UserId) return BadRequest();
        var result = await SendCommandAsync<RemoveUserRoleCommand, RemoveUserRoleCommand, Boolean>(request);
        return Ok(result);
    }

    [Authorize(Roles = ApplicationRoles.Admin)]
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<bool>> RemoveUser([FromRoute] Guid id)
    {
        var request = new DeleteUserCommand { UserId = id };
        var result = await SendCommandAsync<DeleteUserCommand, DeleteUserCommand, Boolean>(request);
        return Ok(result);
    }

    [Authorize(Roles = ApplicationRoles.Admin)]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetUserById([FromRoute] Guid id)
    {
        var request = new GetUserByIdQuery { UserId = id };
        var result = await SendQueryAsync<GetUserByIdQuery, UserDto>(request);
        return Ok(result);
    }

    [Authorize(Roles = ApplicationRoles.Admin)]
    [HttpGet]
    public async Task<ActionResult<PagedResult<UserDto>>> GetUsers(int page, int size)
    {
        var request = new GetAllUsersByPageQuery { Page = page, Size = size };
        var result = await SendQueryAsync<GetAllUsersByPageQuery, PagedResult<UserDto>>(request);
        return Ok(result);
    }
    [Authorize(Roles = ApplicationRoles.Admin)]
    [HttpPut("{id:guid}/reset-password")]
    public async Task<IActionResult> AdminResetPassword([FromRoute]Guid id,[FromBody] AdminResetUserPasswordCommand command)
    {
        if(id != command.UserId) return BadRequest();
        var result = await SendCommandAsync<AdminResetUserPasswordCommand, AdminResetUserPasswordCommand, IdentityResponse>(command);
        return Ok(result);
    }

    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangeOwnPasswordCommand command)
    {
        var result = await SendCommandAsync<ChangeOwnPasswordCommand, ChangeOwnPasswordCommand, IdentityResponse>(command);
        return Ok(result);
    }
}
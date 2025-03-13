using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Server.AspNetCore;
using Saylo.Centrex.Application.Exceptions;
using Saylo.Centrex.Identity.Core.Application.Commands.Identity;
using Saylo.Centrex.Identity.Core.Application.Interfaces;
using Saylo.Centrex.Identity.Core.Application.Models;
using Saylo.Centrex.Infrastructure.MultiTenancy.Middleware;
using Saylo.Centrex.Infrastructure.Web.Apis;
using ForgotPasswordRequest = Saylo.Centrex.Identity.Core.Application.Models.ForgotPasswordRequest;
using ResetPasswordRequest = Saylo.Centrex.Identity.Core.Application.Models.ResetPasswordRequest;

namespace Saylo.Centrex.Identity.Api.Controllers;

public class LoginController : ApiControllerBase
{
    private readonly ITokenClient _tokenClient;

    public LoginController(ITokenClient tokenClient, IMediator mediator, IMapper mapper, ILogger<LoginController> logger) : base(mediator, mapper, logger)
    {
        _tokenClient = tokenClient;
    }
    [HttpPost]
    [IgnoreTenant]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var response = await _tokenClient.GetTokenAsync(model.Username, model.Password);
        return Ok(response);
    }
    [Route("logout")]
    [HttpGet, HttpPost]
    [IgnoreTenant]
    [AllowAnonymous]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        var authProps = new AuthenticationProperties { RedirectUri = "/" };
        return SignOut(authProps, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
    [HttpPost("forgot-password")]
    [IgnoreTenant]
    [AllowAnonymous]
    public async Task<ActionResult<IdentityResponse>> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var result = await SendCommandAsync<ForgotPasswordCommand, ForgotPasswordRequest, IdentityResponse>(request);
        return Ok(result);
    }
    [HttpPost("reset-password")]
    [IgnoreTenant]
    [AllowAnonymous]
    public async Task<ActionResult<IdentityResponse>> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await SendCommandAsync<ResetPasswordCommand, ResetPasswordRequest, IdentityResponse>(request);
        return Ok(result);
    }
}
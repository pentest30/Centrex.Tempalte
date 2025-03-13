using System.Security.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Server.AspNetCore;
using Saylo.Centrex.Identity.Core.Application.Interfaces;
using Saylo.Centrex.Infrastructure.MultiTenancy.Middleware;

namespace Saylo.Centrex.Identity.Api.Controllers;

[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
public class AuthorizationController : Controller
{
    private readonly IOpenIdAuthorizationService _openIdAuthorizationService;

    public AuthorizationController(IOpenIdAuthorizationService openIdAuthorizationService)
    {
        _openIdAuthorizationService = openIdAuthorizationService;
    }
   
    [HttpGet("~/connect/authorize")]
    [HttpPost("~/connect/authorize")]
    [IgnoreAntiforgeryToken]
    [AllowAnonymous]
    [IgnoreTenant]
    public async Task<IActionResult> Authorize()
    {
        try
        {
            var principal = await _openIdAuthorizationService.ValidateAuthorizationRequestAsync(HttpContext);
            return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }
        catch (AuthenticationException)
        {
            return Challenge(
                authenticationSchemes: CookieAuthenticationDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                        Request.HasFormContentType ? Request.Form.ToList() : Request.Query.ToList())
                });
        }
    }
   
    [HttpPost("~/connect/token")]
    [AllowAnonymous]
    [IgnoreTenant]
    public async Task<IActionResult> Token()
    {
        var result = await _openIdAuthorizationService.HandleTokenRequestAsync(HttpContext);
        return result;
    }
}
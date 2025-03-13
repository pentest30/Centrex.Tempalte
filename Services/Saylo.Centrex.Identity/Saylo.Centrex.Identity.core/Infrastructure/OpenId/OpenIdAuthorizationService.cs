using System.Security.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using Saylo.Centrex.Application.Common.Interfaces;
using Saylo.Centrex.Identity.Core.Application.Interfaces;
using Saylo.Centrex.Identity.Core.Domain.Entities.Identity;
using SignInResult = Microsoft.AspNetCore.Mvc.SignInResult;

namespace Saylo.Centrex.Identity.Core.Infrastructure.OpenId;

public class OpenIdAuthorizationService : IOpenIdAuthorizationService
{
    private readonly IOpenIddictScopeManager _scopeManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<OpenIdAuthorizationService> _logger;
    private readonly IIdentityService _identityService;
    private readonly IDateTimeProvider _timeProvider;

    public OpenIdAuthorizationService(
        IOpenIddictScopeManager scopeManager,
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        IIdentityService identityService, 
        IDateTimeProvider timeProvider,
        ILogger<OpenIdAuthorizationService> logger)
    {
        _scopeManager = scopeManager;
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
        _identityService = identityService;
        _timeProvider = timeProvider;
    }

    public async Task<IActionResult> HandleTokenRequestAsync(HttpContext context)
    {
        var request = context.GetOpenIddictServerRequest() ??
                      throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        if (!request.IsPasswordGrantType())
        {
            throw new InvalidOperationException("The specified grant type is not supported.");
        }

        var user = await _identityService.GetUserByUserNameAsync(request.Username);
        if (user == null || !(await _userManager.CheckPasswordAsync(user, request.Password)))
        {
            return new UnauthorizedObjectResult(new OpenIddictResponse
            {
                Error = OpenIddictConstants.Errors.InvalidGrant,
                ErrorDescription = "The username/password combination is invalid."
            });
        }

        var scopes = request.GetScopes();
        var principal = await CreateUserPrincipalAsync(user, scopes);

        principal.SetResources("api");
        principal.SetCreationDate(_timeProvider.UtcNow);
        principal.SetExpirationDate(_timeProvider.UtcNow.AddDays(5));

        foreach (var claim in principal.Claims)
        {
            claim.SetDestinations(GetDestinations(claim));
        }

        _logger.LogInformation("Token issued for user {UserId} with TenantId {TenantId}.",
            user.Id, user.TenantId);

        return new SignInResult(
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            principal);
    }

    public async Task<ClaimsPrincipal> ValidateAuthorizationRequestAsync(HttpContext context)
    {
        var result = await context.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!result.Succeeded)
        {
            throw new AuthenticationException("Authentication failed.");
        }

        // Get user ignoring tenant filter
        var userId = _userManager.GetUserId(result.Principal);
        var user = await _identityService.GetUserByIdAsync(userId, includeRoles: true, ignoreTenantFilter: true);

        if (user == null)
        {
            throw new InvalidOperationException("The user details cannot be retrieved.");
        }

        var principal = await _signInManager.CreateUserPrincipalAsync(user);
        var request = context.GetOpenIddictServerRequest() ??
                      throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        principal.SetScopes(request.GetScopes());

        var resources = new List<string>();
        await foreach (var resource in _scopeManager.ListResourcesAsync(principal.GetScopes()))
        {
            resources.Add(resource);
        }

        principal.SetResources(resources);

        foreach (var claim in principal.Claims)
        {
            claim.SetDestinations(GetDestinations(claim));
        }

        _logger.LogInformation("Authorization validated for user {UserId} with TenantId {TenantId}.",
            user.Id, user.TenantId);

        return principal;
    }

    private IEnumerable<string> GetDestinations(Claim claim)
    {
        return claim.Type switch
        {
            OpenIddictConstants.Claims.Subject => new[]
                { OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken },
            OpenIddictConstants.Claims.Name => new[]
                { OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken },
            OpenIddictConstants.Claims.Email => new[]
                { OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken },
            OpenIddictConstants.Claims.Role => new[]
                { OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken },
            "tenantId" => new[]
                { OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken },
            "fullname" => new[]
                { OpenIddictConstants.Destinations.AccessToken, OpenIddictConstants.Destinations.IdentityToken },
            _ => new[] { OpenIddictConstants.Destinations.AccessToken }
        };
    }

    public async Task<ClaimsPrincipal> CreateUserPrincipalAsync(ApplicationUser user, IEnumerable<string> scopes = null)
    {
        var identity = new ClaimsIdentity(
            authenticationType: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            nameType: OpenIddictConstants.Claims.Name,
            roleType: OpenIddictConstants.Claims.Role);

        identity.AddClaim(OpenIddictConstants.Claims.Subject, user.Id.ToString());
        identity.AddClaim(OpenIddictConstants.Claims.Name, user.UserName);
        identity.AddClaim(OpenIddictConstants.Claims.Email, user.Email);
        identity.AddClaim("tenantId", user.TenantId.ToString());
        identity.AddClaim("fullname", $"{user.FirstName} {user.LastName}");

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            identity.AddClaim(OpenIddictConstants.Claims.Role, role);
        }

        var principal = new ClaimsPrincipal(identity);
        if (scopes != null)
        {
            principal.SetScopes(scopes);
        }

        _logger.LogInformation("Principal created with claims for user {UserId}.", user.Id);

        return principal;
    }
}

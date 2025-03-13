using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saylo.Centrex.Identity.Core.Domain.Entities.Identity;

namespace Saylo.Centrex.Identity.Core.Application.Interfaces;

public interface IOpenIdAuthorizationService
{
    Task<ClaimsPrincipal> ValidateAuthorizationRequestAsync(HttpContext context);
    Task<IActionResult> HandleTokenRequestAsync(HttpContext context);
    Task<ClaimsPrincipal> CreateUserPrincipalAsync(ApplicationUser user, IEnumerable<string> scopes = null);
    
}
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using Saylo.Centrex.Infrastructure.MultiTenancy.Configuration;

namespace Saylo.Centrex.Infrastructure.Web.Filters;

public class ValidateTenantAttribute : ActionFilterAttribute
{
    private readonly string _tenantHeaderKey;

    public ValidateTenantAttribute(IOptions<MultiTenantConfig> options)
    {
        _tenantHeaderKey = options.Value.TenantHeaderKey;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(_tenantHeaderKey, out var tenantHeader))
        {
            context.Result = new ObjectResult(new ErrorResponse(
                "TENANT_REQUIRED",
                $"The {_tenantHeaderKey} header is required"))
            {
                StatusCode = (int)HttpStatusCode.BadRequest
            };
        }
    }
}

public record ErrorResponse(string Code, string Message);

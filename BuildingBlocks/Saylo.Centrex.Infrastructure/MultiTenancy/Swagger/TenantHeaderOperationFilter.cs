using Microsoft.OpenApi.Models;
using Saylo.Centrex.Infrastructure.MultiTenancy.Configuration;
using Saylo.Centrex.Infrastructure.MultiTenancy.Middleware;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Saylo.Centrex.Infrastructure.MultiTenancy.Swagger
{
    public class TenantHeaderOperationFilter : IOperationFilter
    {
        private readonly MultiTenantConfig _config;

        public TenantHeaderOperationFilter(MultiTenantConfig config)
        {
            ArgumentNullException.ThrowIfNull(config);
            _config = config;
        }
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        { 
            var ignoreTenant = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<IgnoreTenantAttribute>()
                .Any();
            if (ignoreTenant)
            {
                return;
            }

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = _config.TenantHeaderKey,
                In = ParameterLocation.Header,
                Required = true,  // Set to true if mandatory
                Description = "Tenant Identifier",
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            });
        }
    }
}
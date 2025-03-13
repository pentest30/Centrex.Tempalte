using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Saylo.Centrex.Infrastructure.Behaviors.Logging;
using Saylo.Centrex.Infrastructure.Behaviors.Validations;

namespace Saylo.Centrex.Infrastructure.Behaviors;

internal static class Startup
{
    internal static IServiceCollection AddBehaviours(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CorrelationIdBehavior<,>));
        return services;
    }
}
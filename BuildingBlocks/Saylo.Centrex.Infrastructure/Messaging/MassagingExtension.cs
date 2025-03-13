using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Saylo.Centrex.Infrastructure.Messaging.Config;

namespace Saylo.Centrex.Infrastructure.Messaging;

public static class MassagingExtension
{
    public static RabbitMqConfigBuilder AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        // Initialize the RabbitMQ config builder
        return new RabbitMqConfigBuilder(services, configuration);
    }
    public static IServiceCollection AddRabbitMq(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<RabbitMqConfigBuilder> configure)
    {
        var builder = new RabbitMqConfigBuilder(services, configuration);
        configure(builder);
        builder.Build();
        return services;
    }
}
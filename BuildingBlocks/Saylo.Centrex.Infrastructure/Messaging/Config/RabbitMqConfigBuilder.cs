using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Saylo.Centrex.Application.Common.Events;
using Saylo.Centrex.Application.Common.Messaging;
using Saylo.Centrex.Application.Common.Messaging.Filters;
using Saylo.Centrex.Application.Exceptions;
using Saylo.Centrex.Infrastructure.Messaging.ConsumerConfiguration;
using Saylo.Centrex.Infrastructure.Messaging.Filters;
using Saylo.Centrex.Infrastructure.Messaging.Options;

namespace Saylo.Centrex.Infrastructure.Messaging.Config;

public class RabbitMqConfigBuilder
{
    private readonly IServiceCollection _services;
    private readonly RabbitMqOptions _options;
    public static readonly Dictionary<string, Type> ConsumersTypes = new();
    public RabbitMqConfigBuilder(
        IServiceCollection services,
        IConfiguration configuration)
    {
        _services = services;
        _options = configuration.GetSection(nameof(RabbitMqOptions)).Get<RabbitMqOptions>();
        ArgumentNullException.ThrowIfNull(_options);
        _services.AddSingleton(_options);
    }

    // Add a single consumer
    public RabbitMqConfigBuilder AddConsumer<T>() where T : class
    {
        var consumerType = typeof(T);
        ConsumersTypes.TryAdd(consumerType.Name, consumerType);
        var handlerInterface = consumerType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageConsumer<>));

        if (handlerInterface != null)
        {
            _services.AddTransient(handlerInterface, consumerType);
            if (_services.Any(service =>
                    service.ServiceType == typeof(IHostedService) &&
                    service.ImplementationType == typeof(MessageHandlingBackgroundService)))
            {
                return this;
            }
      
            RegisterHostedService();
        }
        else
        {
            throw new InvalidOperationException($"The type {consumerType.FullName} does not implement IMessageHandler<>.");
        }

        return this;
    }
    private void RegisterHostedService()
    {
        _services.AddSingleton<RabbitMqReceiver>();
        _services.AddSingleton<IMessageHandlerRegistry, MessageHandlerRegistry>();
        _services.AddHostedService<MessageHandlingBackgroundService>();
        _services.AddTransient<IMessageFilterChainHandler, MessageFilterChainHandler>();
        Console.WriteLine("RabbitMqConsumerService Registered.");
    }
     public RabbitMqConfigBuilder AddMessagePublishers()
    {
        var eventTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => typeof(IIntegrationEvent).IsAssignableFrom(type) && !type.IsInterface)
            .ToList();

        foreach (var eventType in eventTypes)
        {
            var routingKey = _options.RoutingKeys.TryGetValue(eventType.Name, out var key) ? key : null;
            if (routingKey == null)
                throw new RoutingKeyNotFoundException(eventType.Name);

            var senderType = typeof(IMessageSender<>).MakeGenericType(eventType);
            var senderImplementationType = typeof(RabbitMqSender<>).MakeGenericType(eventType);

            _services.AddTransient(senderType, provider =>
            {
                var senderOptions = new RabbitMqSenderOptions
                {
                    HostName = _options.HostName,
                    UserName = _options.UserName,
                    Password = _options.Password,
                    ExchangeName = _options.ExchangeName,
                    RoutingKey = routingKey
                };

                return Activator.CreateInstance(senderImplementationType, senderOptions);
            });
        }

        return this;
    }
    public RabbitMqConfigBuilder AddTenantFilter() 
    {
        _services.AddTransient(typeof(IMessageFilter<>), typeof(TenantMessageFilter<>));
        return this;
    }
    public void Build()
    {
        // Add the message bus
        _services.AddSingleton<IMessageBus, RabbitMqMessageBus>();
    }
}
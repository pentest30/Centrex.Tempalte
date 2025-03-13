using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Saylo.Centrex.Application.Common.Messaging;
using Saylo.Centrex.Application.Common.Messaging.Filters;
using Saylo.Centrex.Infrastructure.Messaging.Config;

namespace Saylo.Centrex.Infrastructure.Messaging;

public class MessageHandlingBackgroundService : BackgroundService
{
    private readonly RabbitMqReceiver _receiver;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MessageHandlingBackgroundService> _logger;

    public MessageHandlingBackgroundService(
        RabbitMqReceiver receiver,
        IServiceProvider serviceProvider,
        ILogger<MessageHandlingBackgroundService> logger)
    {
        _receiver = receiver;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting message handling background service.");

        try
        {
            var consumerTypes = RabbitMqConfigBuilder.ConsumersTypes.Values;
            foreach (var consumerType in consumerTypes)
            {
                var messageType = GetMessageType(consumerType);

                _logger.LogDebug("Registering handler for message type: {MessageType}", messageType.Name);
                dynamic handler = CreateHandlerDelegate(messageType); 
                RegisterHandler(messageType, handler);
            }

            _receiver.StartConsuming();
            _logger.LogInformation("RabbitMQ receiver started consuming messages.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during the setup of message handlers.");
            throw;
        }

        return Task.CompletedTask;
    }

    private void RegisterHandler(Type messageType, dynamic handler)
    {
        typeof(RabbitMqReceiver)
            .GetMethod(nameof(RabbitMqReceiver.RegisterHandler))
            ?.MakeGenericMethod(messageType)
            .Invoke(_receiver, new[] { handler });
    }

    private static Type GetMessageType(Type consumerType)
    {
        var messageType = consumerType.GetInterfaces()
            .First(i => i.IsGenericType && 
                        i.GetGenericTypeDefinition() == typeof(IMessageConsumer<>))
            .GetGenericArguments()[0];
        return messageType;
    }

    private Delegate CreateHandlerDelegate(Type messageType)
    {
        _logger.LogDebug("Creating handler delegate for message type: {MessageType}", messageType.Name);

        // Create Func<T, MetaData, Task>
        var handlerType = typeof(Func<,,>).MakeGenericType(messageType, typeof(MetaData), typeof(Task));
        var method = GetType()
            .GetMethod(nameof(HandleMessage), BindingFlags.NonPublic | BindingFlags.Instance)
            ?.MakeGenericMethod(messageType);

        if (method != null) return Delegate.CreateDelegate(handlerType, this, method);
        _logger.LogError("Failed to create handler delegate: Method not found for message type {MessageType}", messageType.Name);
        throw new InvalidOperationException($"Method not found for message type {messageType.Name}");
    }

    private async Task HandleMessage<T>(T data, MetaData metadata) where T : class
    {
        _logger.LogInformation("Handling message of type {MessageType}.", typeof(T).Name);

        using var scope = _serviceProvider.CreateScope();
        var consumers = scope.ServiceProvider.GetServices<IMessageConsumer<T>>();

        _logger.LogDebug("Found {ConsumerCount} consumers for message type {MessageType}.", consumers.Count(), typeof(T).Name);

        var message = new Message<T>(data, metadata);

        var filterChainHandler = scope.ServiceProvider.GetRequiredService<IMessageFilterChainHandler>();
        var messageContext = new MessageFilterContext<object>(message, metadata, CancellationToken.None);
        await filterChainHandler.ExecuteFilterChainAsync(messageContext, CancellationToken.None);
        
        foreach (var consumer in consumers)
        {
            _logger.LogInformation("Invoking consumer {ConsumerType} for message type {MessageType}.", consumer.GetType().Name, typeof(T).Name);
            await consumer.ConsumeAsync(message, CancellationToken.None);
        }
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping message handling background service.");

        _receiver.Dispose();

        _logger.LogInformation("RabbitMQ receiver disposed.");
        return base.StopAsync(cancellationToken);
    }
}
using Saylo.Centrex.Application.Common.Messaging;

namespace Saylo.Centrex.Infrastructure.Messaging.ConsumerConfiguration;

public class MessageHandlerRegistry : IMessageHandlerRegistry
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();

    public void RegisterHandler(Type messageType, Delegate handler)
    {
        if (!_handlers.ContainsKey(messageType))
        {
            _handlers[messageType] = new List<Delegate>();
        }
        _handlers[messageType].Add(handler);
    }

    public IEnumerable<Delegate> GetHandlers(Type messageType)
    {
        return _handlers.TryGetValue(messageType, out var handlers) 
            ? handlers 
            : Enumerable.Empty<Delegate>();
    }
}
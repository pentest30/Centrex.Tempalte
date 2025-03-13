namespace Saylo.Centrex.Application.Common.Messaging;

public interface IMessageHandlerRegistry
{
    void RegisterHandler(Type messageType, Delegate handler);
    IEnumerable<Delegate> GetHandlers(Type messageType);}
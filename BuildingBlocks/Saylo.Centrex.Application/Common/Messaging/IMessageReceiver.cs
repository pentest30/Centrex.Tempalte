namespace Saylo.Centrex.Application.Common.Messaging;

public interface IMessageReceiver<T>
{
    void Receive(Action<T, MetaData> action);
}
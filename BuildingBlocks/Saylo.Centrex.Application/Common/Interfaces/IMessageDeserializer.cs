namespace Saylo.Centrex.Application.Common.Interfaces;

public interface IMessageDeserializer
{
    dynamic? Deserialize(string message, string eventType);
}
using System.Text.Json;
using Saylo.Centrex.Application.Common.Interfaces;
using Saylo.Centrex.Application.Exceptions;

namespace Saylo.Centrex.Infrastructure.Services;

public class JsonMessageDeserializer : IMessageDeserializer
{
    public dynamic? Deserialize(string message, string eventType)
    {
        var messageType = Type.GetType(eventType) ?? 
                          throw new TypeResolutionException($"Cannot resolve type: {eventType}");

        var result = JsonSerializer.Deserialize(message, messageType);
        return result;
    }
}
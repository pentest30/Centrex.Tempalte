using System.Runtime.Serialization;

namespace Saylo.Centrex.Application.Exceptions;

public class RoutingKeyNotFoundException : Exception
{
    public string EventTypeName { get; }

    public RoutingKeyNotFoundException(string eventTypeName)
        : base($"Routing key not found for event type: {eventTypeName}")
    {
        EventTypeName = eventTypeName;
    }

    public RoutingKeyNotFoundException(string eventTypeName, Exception innerException)
        : base($"Routing key not found for event type: {eventTypeName}", innerException)
    {
        EventTypeName = eventTypeName;
    }

    // Add serialization constructor for proper serialization across AppDomains
    protected RoutingKeyNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        EventTypeName = info.GetString(nameof(EventTypeName));
    }

    // Override GetObjectData to ensure proper serialization
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(EventTypeName), EventTypeName);
    }
}

namespace Saylo.Centrex.Domain.Entities;

public class OutboxEvent : AggregateRoot<Guid>
{
    public string EventType { get; set; }
    public string Payload { get; set; }
    public DateTime? PublishedDate { get; set; }
    public string? ProcessingId { get; set; }
    public string? Error { get; set; }
    public OutboxStatus Status { get; set; }
    public string? ObjectId { get; set; }
}

public enum OutboxStatus
{
    Created,
    Processing,
    Published,
    Failed
}
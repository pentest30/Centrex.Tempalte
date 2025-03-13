namespace Saylo.Centrex.Application.Common.Messaging;

public class MetaData
{
    public MetaData()
    {
    }
    public MetaData(string correlationId, string messageId, Guid? tenantId, DateTimeOffset creationDateTime)
    {
        CreationDateTime = creationDateTime;
        CorrelationId = correlationId;
        TenantId = tenantId;
        MessageId = messageId;
        EnqueuedDateTime = DateTimeOffset.Now;
    }
    public string MessageId { get; set; }

    public string MessageVersion { get; set; }

    public string CorrelationId { get; set; }

    public DateTimeOffset? CreationDateTime { get; set; }

    public DateTimeOffset? EnqueuedDateTime { get; set; }
    public Guid? TenantId { get; set; }
}
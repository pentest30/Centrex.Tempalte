namespace Saylo.Centrex.Application.Common.Events;

public interface IIntegrationEvent
{
    public object Id { get; set; }
    public string? CorrelationId { get; set; }
    public Guid? TransactionId { get; set; }
    public Guid? TenantId { get; set; }
}
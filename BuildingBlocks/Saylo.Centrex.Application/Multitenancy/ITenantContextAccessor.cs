namespace Saylo.Centrex.Application.Multitenancy;

public interface ITenantContextAccessor
{
    Guid? TenantId { get; set; }
    public string? MultiTenantConnectionString { get; set; }
}
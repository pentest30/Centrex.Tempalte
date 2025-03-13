namespace Saylo.Centrex.Application.Multitenancy;

public interface ITenantCacheStore 
{
    Task SetTenantAsync(string tenantId, TenantInfoDto tenant);
    Task<TenantInfoDto?> GetTenantAsync(string tenantId);
    Task<bool> ValidateUserTenantAccessAsync( string targetTenantId);
    Task<bool> RemoveTenantAsync(string tenantId);
}

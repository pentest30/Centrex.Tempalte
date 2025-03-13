using Saylo.Centrex.Domain.Entities;

namespace Saylo.Centrex.Application.Multitenancy;

public interface ITenantStore<TTenant, TContext>
    where TTenant : TenantInfoBase
{
    Task<string?> GetConnectionStringForTenantAsync(string tenantId);
    Task<TTenant?> GetTenantAsync(string? tenantId);
}
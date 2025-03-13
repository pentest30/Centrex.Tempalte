using System.Text.Json;
using Saylo.Centrex.Application.Common.Identity;
using Saylo.Centrex.Application.Multitenancy;
using StackExchange.Redis;

namespace Saylo.Centrex.Infrastructure.MultiTenancy.Core;

public class RedisTenantCacheStore : ITenantCacheStore
{
    private readonly IDatabase _db;
    private const string TenantKeyPrefix = "tenant:";

    public RedisTenantCacheStore(
        IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task SetTenantAsync(string tenantId, TenantInfoDto tenant)
    {
        var key = $"{TenantKeyPrefix}{tenantId.ToLower()}";
        await _db.StringSetAsync(key, JsonSerializer.Serialize(tenant));
    }

    public async Task<TenantInfoDto?> GetTenantAsync(string tenantId)
    {
        var key = $"{TenantKeyPrefix}{tenantId.ToLower()}";
        var value = await _db.StringGetAsync(key);
        return value.IsNull ? null : JsonSerializer.Deserialize<TenantInfoDto>(value);
    }
    public async Task<bool> ValidateUserTenantAccessAsync(string targetTenantId)
    {
        var currentTenant = await GetTenantAsync(targetTenantId);
        if (currentTenant == null) return false;
        if (currentTenant.ParentId == null && string.Equals(currentTenant.Id, targetTenantId, StringComparison.OrdinalIgnoreCase))
            return true;
        // Traverse up the tenant hierarchy to find if target is under user's primary tenant (case insensitive comparison)
        while (!string.IsNullOrEmpty(currentTenant.ParentId))
        {
            if (string.Equals(currentTenant.ParentId, targetTenantId, StringComparison.OrdinalIgnoreCase))
                return true;

            currentTenant = await GetTenantAsync(currentTenant.ParentId);
            if (currentTenant == null) break;
        }

        return false;
    }

    public async Task<bool> RemoveTenantAsync(string tenantId)
    {
        var key = $"{TenantKeyPrefix}{tenantId}";
        return await _db.KeyDeleteAsync(key);
    }
}
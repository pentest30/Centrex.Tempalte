namespace Saylo.Centrex.Infrastructure.MultiTenancy.Configuration;

public class MultiTenantConfig
{
    public string? DefaultConnectionString { get; set; } = string.Empty;
    public string TenantHeaderKey { get; set; } = "X-Tenant-Id"; // Default header for tenant ID
    public bool ApplyMigrationsOnStartup { get; set; } = true;
    public string DefaultTenantId => Guid.Parse("bdcb855f-a6a0-444f-997e-0004603d6c93").ToString();
}

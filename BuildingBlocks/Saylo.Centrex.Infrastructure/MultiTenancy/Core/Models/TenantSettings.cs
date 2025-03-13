namespace Saylo.Centrex.Infrastructure.MultiTenancy.Core.Models;

public class TenantSettings
{
    public string InitialTenantId { get; set; }
    public string InitialTenantName { get; set; }
    public string InitialConnectionString { get; set; }
    public bool IsInitialTenantActive { get; set; }
}

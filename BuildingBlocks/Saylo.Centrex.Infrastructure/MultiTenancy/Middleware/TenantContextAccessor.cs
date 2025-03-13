using Saylo.Centrex.Application.Multitenancy;

namespace Saylo.Centrex.Infrastructure.MultiTenancy.Middleware;

    public class TenantContextAccessor : ITenantContextAccessor
    {
        public Guid? TenantId { get; set; }
        public string? MultiTenantConnectionString { get; set; }
    }


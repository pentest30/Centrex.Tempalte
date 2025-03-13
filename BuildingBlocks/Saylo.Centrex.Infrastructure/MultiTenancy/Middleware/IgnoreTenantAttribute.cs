namespace Saylo.Centrex.Infrastructure.MultiTenancy.Middleware;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class IgnoreTenantAttribute : Attribute;
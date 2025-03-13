using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Saylo.Centrex.Application.Extensions;
using Saylo.Centrex.Application.Multitenancy;
using Saylo.Centrex.Domain.Entities;

namespace Saylo.Centrex.Infrastructure.MultiTenancy.Core;

public class MultiTenantContextBase
{
    private readonly ITenantContextAccessor _tenantContextAccessor;

    public MultiTenantContextBase(ITenantContextAccessor tenantContextAccessor)
    {
        _tenantContextAccessor = tenantContextAccessor;
    }

    private Guid? TenantId => _tenantContextAccessor.TenantId ;

    public void ConfigureMultiTenancy(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ITenantEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = GetType()
                    .GetMethod(nameof(SetTenantFilter), BindingFlags.NonPublic | BindingFlags.Instance)!
                    .MakeGenericMethod(entityType.ClrType);

                method.Invoke(this, new object[] { modelBuilder });
            }
        }
    }

    public void SetTenantIdForEntities(ChangeTracker changeTracker)
    {
        var entities = changeTracker.Entries<ITenantEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entity in entities)
        {
            if (entity.Entity.TenantId.IsNullOrEmpty())
            {
                entity.Entity.TenantId = TenantId;
            }
        }
    }
    private void SetTenantFilter<T>(ModelBuilder modelBuilder) where T : class, ITenantEntity
    {
        if (TenantId.IsNullOrEmpty())
        {
            throw new InvalidOperationException("TenantId is not available. Ensure tenant context is set.");
        }

        modelBuilder.Entity<T>().HasQueryFilter(e => e.TenantId == TenantId);
    }
}

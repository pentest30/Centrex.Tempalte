using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Saylo.Centrex.Application.Common.Events;
using Saylo.Centrex.Application.Common.Interfaces;
using Saylo.Centrex.Application.Multitenancy;
using Saylo.Centrex.Identity.Core.Domain.Entities.Identity;
using Saylo.Centrex.Infrastructure.Persistence;

namespace Saylo.Centrex.Identity.Core.Infrastructure.Persistence;
public class SayloIdentityDbContext : MultiTenantIdentityBaseDbContext<ApplicationUser, AdminRole, Guid, AdminUserClaim, AdminUserRole, AdminUserLogin, AdminRoleClaim ,AdminUserToken, SayloIdentityDbContext>
{
    private readonly ITenantContextAccessor _tenantContextAccessor;

    public SayloIdentityDbContext(DbContextOptions options, ITenantContextAccessor tenantContextAccessor, IEventPublisher eventPublisher, IIntegrationEventPublisherService integrationEventPublisherService) : base(options, tenantContextAccessor, eventPublisher, integrationEventPublisherService)
    {
        _tenantContextAccessor = tenantContextAccessor;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.UseOpenIddict();
        var assembly = Assembly.GetExecutingAssembly();
        builder.ApplyConfigurationsFromAssembly(assembly);
        builder.Entity<ApplicationUser>()
            .HasQueryFilter(u => u.TenantId == _tenantContextAccessor.TenantId);
       
    }
}
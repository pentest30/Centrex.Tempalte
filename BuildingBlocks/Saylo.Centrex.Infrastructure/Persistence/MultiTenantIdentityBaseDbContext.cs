using System.Data;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Saylo.Centrex.Application.Common.Events;
using Saylo.Centrex.Application.Common.Interfaces;
using Saylo.Centrex.Application.Multitenancy;
using Saylo.Centrex.Domain.Entities;
using Saylo.Centrex.Domain.Repositories;
using Saylo.Centrex.Infrastructure.MultiTenancy.Core;

namespace Saylo.Centrex.Infrastructure.Persistence;
public abstract class MultiTenantIdentityBaseDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken, TDbContext> 
    : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>, 
        IUnitOfWork
    where TUser : IdentityUser<TKey>, ITenantEntity
    where TRole : IdentityRole<TKey>
    where TKey : IEquatable<TKey>
    where TUserClaim : IdentityUserClaim<TKey>
    where TUserRole : IdentityUserRole<TKey>
    where TUserLogin : IdentityUserLogin<TKey>
    where TRoleClaim : IdentityRoleClaim<TKey>
    where TUserToken : IdentityUserToken<TKey>
    where TDbContext : DbContext
{
    private readonly MultiTenantContextBase _multiTenantBase;
    private readonly IEventPublisher _eventPublisher;
    private readonly IIntegrationEventPublisherService _integrationEventPublisherService;
    private IDbContextTransaction _dbContextTransaction;

    protected MultiTenantIdentityBaseDbContext(
        DbContextOptions options,
        ITenantContextAccessor tenantContextAccessor,
        IEventPublisher eventPublisher,
        IIntegrationEventPublisherService integrationEventPublisherService)
        : base(options)
    {
        _eventPublisher = eventPublisher;
        _integrationEventPublisherService = integrationEventPublisherService;
        _multiTenantBase = new MultiTenantContextBase(tenantContextAccessor);
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        _multiTenantBase.ConfigureMultiTenancy(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

   
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var transactionId = Guid.NewGuid();
        await this.DispatchDomainEventsAsync(transactionId, _eventPublisher, cancellationToken: cancellationToken);
        _multiTenantBase.SetTenantIdForEntities(ChangeTracker);
        var row = await base.SaveChangesAsync(cancellationToken);
        await _integrationEventPublisherService.PublishIntegrationEventAsync(this, transactionId, cancellationToken);
        return row;
    }

    public async Task<IDisposable> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
    {
        _dbContextTransaction = await Database.BeginTransactionAsync(isolationLevel, cancellationToken);
        return _dbContextTransaction;
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await _dbContextTransaction.CommitAsync(cancellationToken);
    }
    
}
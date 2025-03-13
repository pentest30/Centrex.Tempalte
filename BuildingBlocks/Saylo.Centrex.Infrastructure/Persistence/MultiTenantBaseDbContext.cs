using System.Data;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Saylo.Centrex.Application.Common.Events;
using Saylo.Centrex.Application.Common.Interfaces;
using Saylo.Centrex.Application.Multitenancy;
using Saylo.Centrex.Domain.Entities;
using Saylo.Centrex.Domain.Repositories;
using Saylo.Centrex.Infrastructure.MultiTenancy.Core;

namespace Saylo.Centrex.Infrastructure.Persistence;

public abstract class MultiTenantBaseDbContext<TDbContext> : DbContext, IUnitOfWork where TDbContext : DbContext
{
    private readonly IEventPublisher _eventPublisher;
    private readonly IIntegrationEventPublisherService _integrationEventPublisherService;
    private IDbContextTransaction _dbContextTransaction;
    private readonly MultiTenantContextBase _multiTenantBase;
    protected MultiTenantBaseDbContext(
        DbContextOptions<TDbContext> options,
        ITenantContextAccessor tenantContextAccessor, 
        IEventPublisher eventPublisher, 
        IIntegrationEventPublisherService integrationEventPublisherService)
        : base(options)
    {
        _eventPublisher = eventPublisher;
        _integrationEventPublisherService = integrationEventPublisherService;
        _multiTenantBase = new MultiTenantContextBase(tenantContextAccessor);
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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        _multiTenantBase.ConfigureMultiTenancy(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
using Microsoft.EntityFrameworkCore;
using Saylo.Centrex.Application.Common.Specifications;
using Saylo.Centrex.Domain.Entities;
using Saylo.Centrex.Domain.Repositories;
using Saylo.Centrex.Domain.Specifications;

namespace Saylo.Centrex.Infrastructure.Persistence;

public class DbContextReadRepository<TDbContext, TEntity> : IReadRepository<TEntity> 
    where TEntity : class, IAggregateRoot
    where TDbContext : DbContext
{
    private readonly IProjectionProvider _projectionProvider;
    private readonly DbSet<TEntity> _dbSet;

    public DbContextReadRepository(TDbContext context, IProjectionProvider projectionProvider)
    {
        _projectionProvider = projectionProvider ?? throw new ArgumentNullException(nameof(projectionProvider));
        _dbSet = context.Set<TEntity>();
    }

    public async Task<TEntity> FirstOrDefaultAsync(ISpecification<TEntity> spec , CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(spec);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<TEntity>> ListAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(spec);
        return await query.ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task<int> CountAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default)
    {
        var query = ApplySpecification(spec);
        return await query.CountAsync(cancellationToken: cancellationToken);
    }

    public async Task<TResult> FirstOrDefaultAsync<TResult>(ISpecification<TEntity, TResult> spec, CancellationToken cancellationToken = default)
    {
        var evaluator = new SpecificationEvaluator<TEntity, TResult>(_projectionProvider);
        var query = evaluator.GetQuery(_dbSet.AsQueryable().AsNoTracking(), spec);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<TResult>> ListAsync<TResult>(ISpecification<TEntity, TResult> spec, CancellationToken cancellationToken = default)
    {
        var evaluator = new SpecificationEvaluator<TEntity, TResult>(_projectionProvider);
        var query = evaluator.GetQuery(_dbSet.AsQueryable().AsNoTracking(), spec);
        return await query.ToListAsync(cancellationToken);
    }

    private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
    {
        return SpecificationEvaluator<TEntity>.GetQuery(_dbSet.AsQueryable().AsNoTracking(), spec);
    }
}
using Saylo.Centrex.Domain.Entities;
using Saylo.Centrex.Domain.Specifications;

namespace Saylo.Centrex.Domain.Repositories;

public interface IReadRepository<TEntity> where TEntity : class, IAggregateRoot
{
    Task<TEntity> FirstOrDefaultAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);
    Task<List<TEntity>> ListAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);
    Task<int> CountAsync(ISpecification<TEntity> spec, CancellationToken cancellationToken = default);
    Task<TResult> FirstOrDefaultAsync<TResult>(ISpecification<TEntity, TResult> spec, CancellationToken cancellationToken = default);
    Task<List<TResult>> ListAsync<TResult>(ISpecification<TEntity, TResult> spec, CancellationToken cancellationToken = default);
}
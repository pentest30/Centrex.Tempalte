using Saylo.Centrex.Application.Common.Queries;
using Saylo.Centrex.Domain.Entities;
using Saylo.Centrex.Domain.Repositories;
using Saylo.Centrex.Domain.Specifications;

namespace Saylo.Centrex.Application.Extensions;

public static class RepositoryExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T, TEntity>(
        this IReadRepository<TEntity> repository,
        ISpecification<TEntity, T> dataSpecification,
        ISpecification<TEntity> countSpecification,
        CancellationToken cancellationToken = default) 
        where T : class
        where TEntity : class, IAggregateRoot
    {
        var data = await repository.ListAsync(dataSpecification, cancellationToken);
        var count = await repository.CountAsync(countSpecification, cancellationToken);
        return new PagedResult<T>(data, count);
    }
}
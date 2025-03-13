using Saylo.Centrex.Application.Common.Specifications;
using Saylo.Centrex.Domain.Entities;

namespace Saylo.Centrex.Application.Extensions;

public static class PaginationExtensions
{
    public static SpecificationBuilder<T, TResult> Paging<T, TResult>(
        this SpecificationBuilder<T, TResult> specification,
        int page,
        int size) where T : class, IAggregateRoot
    {
        var validPage = Math.Max(1, page);
        var validSize = Math.Max(1, size);
        
        var skip = (validPage - 1) * validSize;
        return specification.Skip(skip).Take(validSize);
    }


    public static IQueryable<T> Paging<T>(
        this IQueryable<T> specification,
        int page,
        int size) where T : class, IAggregateRoot
    {
        var validPage = Math.Max(1, page);
        var validSize = Math.Max(1, size);

        var skip = (validPage - 1) * validSize;
        return specification.Skip(skip).Take(validSize);
    }
}
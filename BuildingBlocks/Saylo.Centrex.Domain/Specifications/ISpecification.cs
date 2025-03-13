using System.Linq.Expressions;

namespace Saylo.Centrex.Domain.Specifications;

public interface ISpecification<T>
{
    Expression<Func<T, bool>> Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    List<string> IncludeStrings { get; }
    Expression<Func<T, object>> OrderBy { get; }
    Expression<Func<T, object>> OrderByDescending { get; }
    List<Expression<Func<T, object>>> ThenByExpressions { get; }
    List<Expression<Func<T, object>>> ThenByDescendingExpressions { get; }
    bool IsPagingEnabled { get; }
    bool AsNoTrackingEnabled { get; }
    bool IgnoreQueryFiltersEnabled { get; }
    bool SplitQueryEnabled { get; }
    int Take { get; }
    int Skip { get; }
}

public interface ISpecification<T, TResult> : ISpecification<T>
{
    Expression<Func<T, TResult>> Selector { get; }
    bool UseProjection { get; }
}
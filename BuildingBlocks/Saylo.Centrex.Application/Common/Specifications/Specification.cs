using System.Linq.Expressions;
using Saylo.Centrex.Domain.Specifications;

namespace Saylo.Centrex.Application.Common.Specifications;

public class Specification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>> Criteria { get; private set; }
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public List<string> IncludeStrings { get; } = new();
    public Expression<Func<T, object>> OrderBy { get; private set; }
    public Expression<Func<T, object>> OrderByDescending { get; private set; }
    public List<Expression<Func<T, object>>> ThenByExpressions { get; } = new();
    public List<Expression<Func<T, object>>> ThenByDescendingExpressions { get; } = new();
    public bool IsPagingEnabled { get; private set; }
    public bool AsNoTrackingEnabled { get; private set; }
    public bool IgnoreQueryFiltersEnabled { get; private set; }
    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool SplitQueryEnabled { get; private set; }

    protected void AddCriteria(Expression<Func<T, bool>> criteria) => Criteria = criteria;
    protected void AddInclude(Expression<Func<T, object>> include) => Includes.Add(include);
    protected void AddInclude(string include) => IncludeStrings.Add(include);
    protected void ApplyPaging(int skip, int take) => (Skip, Take, IsPagingEnabled) = (skip, take, true);
    protected void ApplyOrderBy(Expression<Func<T, object>> orderBy) => OrderBy = orderBy;
    protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDesc) => OrderByDescending = orderByDesc;
    protected void AddThenBy(Expression<Func<T, object>> thenBy) => ThenByExpressions.Add(thenBy);

    protected void AddThenByDescending(Expression<Func<T, object>> thenByDesc) =>
        ThenByDescendingExpressions.Add(thenByDesc);

    protected void EnableAsNoTracking() => AsNoTrackingEnabled = true;
    protected void EnableIgnoreQueryFilters() => IgnoreQueryFiltersEnabled = true;
    protected void EnableAsSplitQuery() => SplitQueryEnabled = true;
}

// Specification with projection
public class Specification<T, TResult> : Specification<T>, ISpecification<T, TResult>
{
    public Expression<Func<T, TResult>> Selector { get; private set; }
    public bool UseProjection { get; private set; }

    protected void ApplyProjection(Expression<Func<T, TResult>> selector)
    {
        Selector = selector;
        UseProjection = true;
    }
}
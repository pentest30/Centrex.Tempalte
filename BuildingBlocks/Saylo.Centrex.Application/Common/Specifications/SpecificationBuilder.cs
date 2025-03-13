using System.Linq.Expressions;
using Saylo.Centrex.Domain.Entities;
using Saylo.Centrex.Domain.Specifications;

namespace Saylo.Centrex.Application.Common.Specifications;

public class SpecificationBuilder<T> where T : class, IAggregateRoot
{
    private readonly Specification<T> _specification;

    public SpecificationBuilder() => _specification = new InternalSpecification<T>();

    public SpecificationBuilder<T> Where(Expression<Func<T, bool>> criteria)
    {
        (_specification as InternalSpecification<T>)?.AddCriteria(criteria);
        return this;
    }

    public SpecificationBuilder<T> Include(Expression<Func<T, object>> includeExpression)
    {
        (_specification as InternalSpecification<T>)?.AddInclude(includeExpression);
        return this;
    }

    public SpecificationBuilder<T> IncludeString(string includeString)
    {
        (_specification as InternalSpecification<T>)?.AddInclude(includeString);
        return this;
    }

    public SpecificationBuilder<T> OrderBy(Expression<Func<T, object>> orderByExpression)
    {
        (_specification as InternalSpecification<T>)?.ApplyOrderBy(orderByExpression);
        return this;
    }

    public SpecificationBuilder<T> OrderByDescending(Expression<Func<T, object>> orderByDescExpression)
    {
        (_specification as InternalSpecification<T>)?.ApplyOrderByDescending(orderByDescExpression);
        return this;
    }

    public SpecificationBuilder<T> ThenBy(Expression<Func<T, object>> orderByExpression)
    {
        (_specification as InternalSpecification<T>)?.AddThenBy(orderByExpression);
        return this;
    }

    public SpecificationBuilder<T> ThenByDescending(Expression<Func<T, object>> orderByDescExpression)
    {
        (_specification as InternalSpecification<T>)?.AddThenByDescending(orderByDescExpression);
        return this;
    }

    public SpecificationBuilder<T> ApplyPaging(int skip, int take)
    {
        (_specification as InternalSpecification<T>)?.ApplyPaging(skip, take);
        return this;
    }

    public SpecificationBuilder<T> Take(int take)
    {
        var currentSkip = _specification.Skip;
        (_specification as InternalSpecification<T>)?.ApplyPaging(currentSkip, take);
        return this;
    }

    public SpecificationBuilder<T> Skip(int skip)
    {
        var currentTake = _specification.Take == 0 ? int.MaxValue : _specification.Take;
        (_specification as InternalSpecification<T>)?.ApplyPaging(skip, currentTake);
        return this;
    }

    public SpecificationBuilder<T> AsNoTracking()
    {
        (_specification as InternalSpecification<T>)?.EnableAsNoTracking();
        return this;
    }
    public SpecificationBuilder<T> AsSplitQuery()
    {
        (_specification as InternalSpecification<T>)?.EnableAsSplitQuery();
        return this;
    }
    public SpecificationBuilder<T> IgnoreQueryFilters()
    {
        (_specification as InternalSpecification<T>)?.EnableIgnoreQueryFilters();
        return this;
    }

    public ISpecification<T> Build() => _specification;

    private class InternalSpecification<TEntity> : Specification<TEntity>
    {
        public new void AddCriteria(Expression<Func<TEntity, bool>> criteria) => base.AddCriteria(criteria);
        public new void AddInclude(Expression<Func<TEntity, object>> include) => base.AddInclude(include);
        public new void AddInclude(string include) => base.AddInclude(include);
        public new void ApplyPaging(int skip, int take) => base.ApplyPaging(skip, take);
        public new void ApplyOrderBy(Expression<Func<TEntity, object>> orderBy) => base.ApplyOrderBy(orderBy);
        public new void ApplyOrderByDescending(Expression<Func<TEntity, object>> orderByDesc) => base.ApplyOrderByDescending(orderByDesc);
        public new void AddThenBy(Expression<Func<TEntity, object>> thenBy) => base.AddThenBy(thenBy);
        public new void AddThenByDescending(Expression<Func<TEntity, object>> thenByDesc) => base.AddThenByDescending(thenByDesc);
        public new void EnableAsNoTracking() => base.EnableAsNoTracking();
        public new void EnableIgnoreQueryFilters() => base.EnableIgnoreQueryFilters();

        public new void EnableAsSplitQuery() => base.EnableAsSplitQuery(); 
    }
}
public class SpecificationBuilder<T, TResult> where T : class,  IAggregateRoot
{
    private readonly Specification<T, TResult> _specification;
    private readonly IProjectionProvider _projectionProvider;

    public SpecificationBuilder(IProjectionProvider projectionProvider)
    {
        _specification = new InternalSpecification<T, TResult>();
        _projectionProvider = projectionProvider;
    }

    public SpecificationBuilder<T, TResult> Where(Expression<Func<T, bool>> criteria)
    {
        (_specification as InternalSpecification<T, TResult>)?.AddCriteria(criteria);
        return this;
    }

    public SpecificationBuilder<T, TResult> Include(Expression<Func<T, object>> includeExpression)
    {
        (_specification as InternalSpecification<T, TResult>)?.AddInclude(includeExpression);
        return this;
    }

    public SpecificationBuilder<T, TResult> IncludeString(string includeString)
    {
        (_specification as InternalSpecification<T, TResult>)?.AddInclude(includeString);
        return this;
    }

    public SpecificationBuilder<T, TResult> OrderBy(Expression<Func<T, object>> orderByExpression)
    {
        (_specification as InternalSpecification<T, TResult>)?.ApplyOrderBy(orderByExpression);
        return this;
    }

    public SpecificationBuilder<T, TResult> OrderByDescending(Expression<Func<T, object>> orderByDescExpression)
    {
        (_specification as InternalSpecification<T, TResult>)?.ApplyOrderByDescending(orderByDescExpression);
        return this;
    }

    public SpecificationBuilder<T, TResult> ThenBy(Expression<Func<T, object>> orderByExpression)
    {
        (_specification as InternalSpecification<T, TResult>)?.AddThenBy(orderByExpression);
        return this;
    }

    public SpecificationBuilder<T, TResult> ThenByDescending(Expression<Func<T, object>> orderByDescExpression)
    {
        (_specification as InternalSpecification<T, TResult>)?.AddThenByDescending(orderByDescExpression);
        return this;
    }

    public SpecificationBuilder<T, TResult> ApplyPaging(int skip, int take)
    {
        (_specification as InternalSpecification<T, TResult>)?.ApplyPaging(skip, take);
        return this;
    }

    public SpecificationBuilder<T, TResult> Take(int take)
    {
        var currentSkip = _specification.Skip;
        (_specification as InternalSpecification<T, TResult>)?.ApplyPaging(currentSkip, take);
        return this;
    }

    public SpecificationBuilder<T, TResult> Skip(int skip)
    {
        var currentTake = _specification.Take == 0 ? int.MaxValue : _specification.Take;
        (_specification as InternalSpecification<T, TResult>)?.ApplyPaging(skip, currentTake);
        return this;
    }

    public SpecificationBuilder<T, TResult> AsNoTracking()
    {
        (_specification as InternalSpecification<T, TResult>)?.EnableAsNoTracking();
        return this;
    }
    public SpecificationBuilder<T, TResult> AsSplitQuery()
    {
        (_specification as InternalSpecification<T, TResult>)?.EnableAsSplitQuery();
        return this;
    }

    public SpecificationBuilder<T, TResult> IgnoreQueryFilters()
    {
        (_specification as InternalSpecification<T, TResult>)?.EnableIgnoreQueryFilters();
        return this;
    }

    public SpecificationBuilder<T, TResult> Select(Expression<Func<T, TResult>> selector)
    {
        (_specification as InternalSpecification<T, TResult>)?.ApplyProjection(selector);
        return this;
    }

    public SpecificationBuilder<T, TResult> Project()
    {
        var projection = _projectionProvider.GetProjection<T, TResult>();
        (_specification as InternalSpecification<T, TResult>)?.ApplyProjection(projection);
        return this;
    }

    public ISpecification<T, TResult> Build() => _specification;

    private class InternalSpecification<TEntity, TProjection> : Specification<TEntity, TProjection>
    {
        public new void AddCriteria(Expression<Func<TEntity, bool>> criteria) => base.AddCriteria(criteria);
        public new void AddInclude(Expression<Func<TEntity, object>> include) => base.AddInclude(include);
        public new void AddInclude(string include) => base.AddInclude(include);
        public new void ApplyPaging(int skip, int take) => base.ApplyPaging(skip, take);
        public new void ApplyOrderBy(Expression<Func<TEntity, object>> orderBy) => base.ApplyOrderBy(orderBy);
        public new void ApplyOrderByDescending(Expression<Func<TEntity, object>> orderByDesc) => base.ApplyOrderByDescending(orderByDesc);
        public new void AddThenBy(Expression<Func<TEntity, object>> thenBy) => base.AddThenBy(thenBy);
        public new void AddThenByDescending(Expression<Func<TEntity, object>> thenByDesc) => base.AddThenByDescending(thenByDesc);
        public new void EnableAsNoTracking() => base.EnableAsNoTracking();
        public new void EnableIgnoreQueryFilters() => base.EnableIgnoreQueryFilters();
        public new void ApplyProjection(Expression<Func<TEntity, TProjection>> selector) => base.ApplyProjection(selector);
        public new void EnableAsSplitQuery() => base.EnableAsSplitQuery(); 

    }
}
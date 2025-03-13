using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Saylo.Centrex.Domain.Specifications;

namespace Saylo.Centrex.Application.Common.Specifications;

public class SpecificationEvaluator<T> where T : class
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> spec)
    {
        var query = inputQuery;

        if (spec.IgnoreQueryFiltersEnabled)
        {
            query = query.IgnoreQueryFilters();
        }

        if (spec.Criteria != null)
        {
            query = query.Where(spec.Criteria);
        }

        query = spec.Includes.Aggregate(query,
            (current, include) => current.Include(include));

        query = spec.IncludeStrings.Aggregate(query,
            (current, include) => current.Include(include));

        if (spec.OrderBy != null)
        {
            var orderedQuery = query.OrderBy(spec.OrderBy);
            query = spec.ThenByExpressions.Aggregate(orderedQuery,
                (current, thenBy) => current.ThenBy(thenBy));
        }
        else if (spec.OrderByDescending != null)
        {
            var orderedQuery = query.OrderByDescending(spec.OrderByDescending);
            query = spec.ThenByDescendingExpressions.Aggregate(orderedQuery,
                (current, thenByDesc) => current.ThenByDescending(thenByDesc));
        }

        if (spec.IsPagingEnabled)
        {
            query = query.Skip(spec.Skip)
                .Take(spec.Take);
        }

        if (spec.AsNoTrackingEnabled)
        {
            query = query.AsNoTracking();
        }

        if (spec.SplitQueryEnabled)
        {
            query = query.AsSplitQuery();
        }
        return query;
    }
}
public class SpecificationEvaluator<T, TResult> where T : class
{
    private readonly IProjectionProvider _projectionProvider;

    public SpecificationEvaluator(IProjectionProvider projectionProvider)
    {
        _projectionProvider = projectionProvider ?? throw new ArgumentNullException(nameof(projectionProvider));
    }

    public IQueryable<TResult> GetQuery(IQueryable<T> inputQuery, ISpecification<T, TResult> spec)
    {
        if (inputQuery == null) throw new ArgumentNullException(nameof(inputQuery));
        if (spec == null) throw new ArgumentNullException(nameof(spec));

        // Apply base specification
        var query = SpecificationEvaluator<T>.GetQuery(inputQuery, spec);

        // Get the appropriate selector
        Expression<Func<T, TResult>> selector;
        
        if (spec.UseProjection)
        {
            // If UseProjection is true but no selector is provided, get it from provider
            selector = spec.Selector ?? _projectionProvider.GetProjection<T, TResult>();
        }
        else
        {
            // If UseProjection is false, use the explicit selector or throw
            if (spec.Selector == null)
            {
                throw new InvalidOperationException(
                    "Neither UseProjection nor Selector is set. Either provide a Selector or enable UseProjection.");
            }
            selector = spec.Selector;
        }

        // Apply the selector
        if (selector == null)
        {
            throw new InvalidOperationException(
                "Could not determine projection. Both Selector and ProjectionProvider returned null.");
        }

        return query.Select(selector);
    }
}

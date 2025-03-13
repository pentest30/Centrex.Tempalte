using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Saylo.Centrex.Domain.Specifications;

namespace Saylo.Centrex.Application.Common.ManualMapping;

public class ProjectionProvider : IProjectionProvider
{
    public Expression<Func<TSource, TTarget>> GetProjection<TSource, TTarget>()
    {
        if (typeof(IMapFrom<TSource>).IsAssignableFrom(typeof(TTarget)))
        {
            var target = Activator.CreateInstance<TTarget>();
            var mapFrom = target as IMapFrom<TSource>;
            
            var mappingExp = mapFrom!.Mapping();
            var parameter = Expression.Parameter(typeof(TSource), "x");
            
            var converted = Expression.Convert(
                Expression.Invoke(mappingExp, parameter),
                typeof(TTarget)
            );

            return Expression.Lambda<Func<TSource, TTarget>>(converted, parameter);
        }

        throw new NotImplementedException($"Type {typeof(TTarget)} must implement IMapFrom<{typeof(TSource)}>");
    }

    public TTarget Map<TSource, TTarget>(TSource source)
    {
        var projection = GetProjection<TSource, TTarget>();
        return projection.Compile()(source);
    }
}
using System.Linq.Expressions;

namespace Saylo.Centrex.Domain.Specifications;

public interface IProjectionProvider
{
    Expression<Func<TSource, TTarget>> GetProjection<TSource, TTarget>();
    TTarget Map<TSource, TTarget>(TSource source);
}

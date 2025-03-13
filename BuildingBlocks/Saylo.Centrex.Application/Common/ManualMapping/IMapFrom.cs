using System.Linq.Expressions;

namespace Saylo.Centrex.Application.Common.ManualMapping;

public interface IMapFrom<T>
{
    Expression<Func<T, object>> Mapping();
}
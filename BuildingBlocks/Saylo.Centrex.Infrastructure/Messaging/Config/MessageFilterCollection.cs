using Saylo.Centrex.Application.Common.Messaging.Filters;

namespace Saylo.Centrex.Infrastructure.Messaging.Config;

public  class MessageFilterCollection
{
    protected readonly List<Type> Filters = new();
    protected readonly List<Type> GlobalFilters = new();

    public void AddGlobalFilter(Type filterType)
    {
        GlobalFilters.Add(filterType);
    }

    public IEnumerable<Type> GetFilters()
    {
        return Filters.Concat(GlobalFilters);
    }
}
public class MessageFilterCollection<T> : MessageFilterCollection where T : class
{
    public void AddFilter<TFilter>() where TFilter : IMessageFilter<T>
    {
        Filters.Add(typeof(TFilter));
    }
}
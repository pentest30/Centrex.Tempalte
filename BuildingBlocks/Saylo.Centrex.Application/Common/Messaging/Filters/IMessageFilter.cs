namespace Saylo.Centrex.Application.Common.Messaging.Filters;

public interface IMessageFilter<T>
{
    Task<bool> Handle(MessageFilterContext<T> context, Func<Task<bool>> next);
}
namespace Saylo.Centrex.Application.Common.Messaging.Filters;

public class MessageFilterContext<T>
{
    public T Message { get; set; }
    public MetaData Metadata { get; }
    public CancellationToken CancellationToken { get; }
    public bool IsFiltered { get; set; }

    public MessageFilterContext(T message, MetaData metadata, CancellationToken cancellationToken)
    {
        Message = message;
        Metadata = metadata;
        CancellationToken = cancellationToken;
        IsFiltered = false;
    }
}
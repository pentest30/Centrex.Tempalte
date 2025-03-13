using System.Text;
using System.Text.Json;

namespace Saylo.Centrex.Application.Common.Messaging;
public class Message<T>
{
    public Message(T data, MetaData metaData)
    {
        Data = data;
        MetaData = metaData;
    }

    public Message()
    {
    }
    public MetaData MetaData { get; set; }

    public T Data { get; set; }

    public string SerializeObject()
    {
        return JsonSerializer.Serialize(this);
    }

    public byte[] GetBytes()
    {
        return Encoding.UTF8.GetBytes(SerializeObject());
    }
}
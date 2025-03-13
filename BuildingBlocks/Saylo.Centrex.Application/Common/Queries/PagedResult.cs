namespace Saylo.Centrex.Application.Common.Queries;

public class PagedResult <T>(List<T> data, int count) where T : class
{
    public IEnumerable<T> Data { get; set; } = data;
    public int Count { get; set; } = count;
}
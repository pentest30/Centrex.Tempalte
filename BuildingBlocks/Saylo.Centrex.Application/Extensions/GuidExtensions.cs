namespace Saylo.Centrex.Application.Extensions;

public static class GuidExtensions
{
    public static bool IsNullOrEmpty(this Guid? guid)
    {
        return !guid.HasValue || guid.Value == Guid.Empty;
    }

    public static bool IsNullOrEmpty(this Guid guid)
    {
        return guid == Guid.Empty;
    }
}
namespace Saylo.Centrex.Application.Common.Interfaces;

public interface IDateTimeProvider  : IScopedService
{
    DateTime Now { get; }

    DateTime UtcNow { get; }

    DateTimeOffset OffsetNow { get; }

    DateTimeOffset OffsetUtcNow { get; }
}
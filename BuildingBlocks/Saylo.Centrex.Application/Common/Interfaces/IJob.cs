namespace Saylo.Centrex.Application.Common.Interfaces;

public interface IJob
{
    Task ProcessAsync(CancellationToken cancellationToken = default);
}
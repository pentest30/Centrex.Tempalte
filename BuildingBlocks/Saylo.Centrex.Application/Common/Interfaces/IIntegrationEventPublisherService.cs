using Saylo.Centrex.Domain.Repositories;

namespace Saylo.Centrex.Application.Common.Interfaces;

public interface IIntegrationEventPublisherService
{
    Task PublishIntegrationEventAsync(IUnitOfWork unitOfWork, Guid transactionId, CancellationToken cancellationToken);
}
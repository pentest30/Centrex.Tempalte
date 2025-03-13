using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Saylo.Centrex.Application.Multitenancy;
using Saylo.Centrex.Domain.Repositories;
using Saylo.Centrex.Identity.Core.Domain.Entities;

namespace Saylo.Centrex.Identity.Core.Application.EventHandlers.AdministrationDomains.Tenants;

public class UpdateAdministrationDomainEventHandler : INotificationHandler<ServiceProidevUpdateEvent>
{
    private readonly IRepository<AdministrationDomain, Guid> _repository;
    private readonly ITenantCacheStore _tenantCacheStore;
    private readonly IMapper _mapper;

    public UpdateAdministrationDomainEventHandler(
        IRepository<AdministrationDomain, Guid> repository,
        ITenantCacheStore tenantCacheStore,
        IMapper mapper)
    {
        _repository = repository;
        _tenantCacheStore = tenantCacheStore;
        _mapper = mapper;
    }
    public async Task Handle(ServiceProidevUpdateEvent notification, CancellationToken cancellationToken)
    {
        var tenant = await _repository
            .GetAll()
            .FirstOrDefaultAsync(t => t.Id == notification.Provider.Id, cancellationToken: cancellationToken);
        ArgumentNullException.ThrowIfNull(tenant);
        tenant.UpdateInfo(notification.Provider.Name, notification.Provider.Description,
            notification.Provider.ContactEmail);
        await _repository.UpdateAsync(tenant, cancellationToken);
        await _tenantCacheStore.SetTenantAsync(notification.Provider.TenantId.ToString(), _mapper.Map<TenantInfoDto>(tenant));
    }
}
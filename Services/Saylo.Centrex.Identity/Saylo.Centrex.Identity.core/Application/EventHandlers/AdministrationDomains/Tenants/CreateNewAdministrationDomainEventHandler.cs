using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Saylo.Centrex.Application.Multitenancy;
using Saylo.Centrex.Domain.Repositories;
using Saylo.Centrex.Identity.Core.Domain.DomainEvents.AdministrationDomains;
using Saylo.Centrex.Identity.Core.Domain.Entities;

namespace Saylo.Centrex.Identity.Core.Application.EventHandlers.AdministrationDomains.Tenants;

public class CreateNewAdministrationDomainEventHandler :
    INotificationHandler<EnterpriseCreatedEvent>,
    INotificationHandler<ServiceProviderCreatedEvent>
{
    private readonly IRepository<AdministrationDomain, Guid> _repository;
    private readonly ITenantCacheStore _tenantCacheStore;
    private readonly IMapper _mapper;
    private readonly ITenantContextAccessor _tenantContextAccessor;
    private readonly ILogger<CreateNewAdministrationDomainEventHandler> _logger;

    public CreateNewAdministrationDomainEventHandler(
        IRepository<AdministrationDomain, Guid> repository,
        ITenantCacheStore tenantCacheStore,
        IMapper mapper,
        ITenantContextAccessor tenantContextAccessor,
        ILogger<CreateNewAdministrationDomainEventHandler> logger)
    {
        _repository = repository;
        _tenantCacheStore = tenantCacheStore;
        _mapper = mapper;
        _tenantContextAccessor = tenantContextAccessor;
        _logger = logger;
    }
    public async Task Handle(EnterpriseCreatedEvent notification, CancellationToken cancellationToken)
    {
        var tenant = new AdministrationDomain().CreateNewTenantFormEntreprise(notification.Enterprise, notification.CorrelationId);
        await _repository.AddAsync(tenant, cancellationToken);
        _logger.LogInformation("Tenant created for entreprise {0}", notification.Enterprise.Name);
        await CacheTheNewTenant(tenant);
    }

    public async Task Handle(ServiceProviderCreatedEvent notification, CancellationToken cancellationToken)
    {
        var tenant = new AdministrationDomain().CreateNewTenantFormServiceProvider(notification.ServiceProvider, notification.CorrelationId);
        await _repository.AddAsync(tenant, cancellationToken);
        _logger.LogInformation("Tenant created for service provider {0}", notification.ServiceProvider.Name);
        await CacheTheNewTenant(tenant);
    }

    private async Task CacheTheNewTenant(AdministrationDomain administrationDomain)
    {
        var tenantInfo = _mapper.Map<TenantInfoDto>(administrationDomain);
        tenantInfo.ParentId = _tenantContextAccessor.TenantId.ToString();
        _logger.LogInformation("Adding TenantInfo to TenantCacheStore with ID: {TenantId}", administrationDomain.Id);
        await _tenantCacheStore.SetTenantAsync(administrationDomain.Id.ToString(), tenantInfo);
        _logger.LogInformation("Successfully added TenantInfo to TenantCacheStore.");
    }
}
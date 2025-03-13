using AutoMapper;
using Saylo.Centrex.Application.Multitenancy;
using Saylo.Centrex.Domain.Entities.ValueObjects;
using Saylo.Centrex.Identity.Core.Application.Commands.AdministrationDomains.Enterprises;
using Saylo.Centrex.Identity.Core.Application.Commands.AdministrationDomains.ServiceProviders;
using Saylo.Centrex.Identity.Core.Application.Models;
using Saylo.Centrex.Identity.Core.Domain.Entities;

namespace Saylo.Centrex.Identity.Core.Application.Mappings;

public class AdministrationDomainsMapperProfile : Profile
{
    public AdministrationDomainsMapperProfile()
    {
        CreateMap<CreateServiceProviderCommand, CreateServiceProviderModel>().ReverseMap();
        CreateMap<UpdateServiceProviderModel, UpdateServiceProviderCommand>().ReverseMap();
        CreateMap<CreateEnterpriseCommand, CreateEnterpriseModel>().ReverseMap();
        CreateMap<AddressDto, Address>().ReverseMap();
        CreateMap<AdministrationDomain, TenantInfoDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => 
                src.TenantId.HasValue ? src.TenantId.Value.ToString() : null));
    }
}
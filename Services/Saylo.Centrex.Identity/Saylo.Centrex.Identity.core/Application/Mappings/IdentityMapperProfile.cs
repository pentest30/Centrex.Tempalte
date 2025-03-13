using AutoMapper;
using Saylo.Centrex.Identity.Core.Application.Commands.Identity;
using Saylo.Centrex.Identity.Core.Application.Models;

namespace Saylo.Centrex.Identity.Core.Application.Mappings;

public class IdentityMapperProfile : Profile
{
    public IdentityMapperProfile()
    {
        CreateMap<CreateUserCommande, RegisterRequest>().ReverseMap();
        CreateMap<ForgotPasswordRequest, ForgotPasswordCommand>();
        CreateMap<ResetPasswordRequest, ResetPasswordCommand>();
    }
}
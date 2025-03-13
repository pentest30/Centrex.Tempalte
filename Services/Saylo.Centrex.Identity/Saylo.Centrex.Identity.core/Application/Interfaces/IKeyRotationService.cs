using Saylo.Centrex.Identity.Core.Infrastructure.OpenId.Models;
using Saylo.Centrex.Identity.Core.Infrastructure.Services;

namespace Saylo.Centrex.Identity.Core.Application.Interfaces;

public interface IKeyRotationService
{
    Task<CertificateInfo> GetCurrentCertificateAsync();
    Task<CertificateInfo> GetPreviousCertificateAsync();
}
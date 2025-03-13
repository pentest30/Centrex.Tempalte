using Saylo.Centrex.Identity.Core.Infrastructure.OpenId.Models;

namespace Saylo.Centrex.Identity.Core.Application.Interfaces;

public interface ICertificateMetadataRepository
{
    Task AddAsync(CertificateMetadata metadata);
    Task<CertificateMetadata?> GetActiveCertificateAsync();
    Task<CertificateMetadata?> GetPreviousCertificateAsync();
    Task DeactivateCurrentCertificateAsync();
    Task<IEnumerable<CertificateMetadata>> GetExpiredCertificatesAsync(int keepCount);
}
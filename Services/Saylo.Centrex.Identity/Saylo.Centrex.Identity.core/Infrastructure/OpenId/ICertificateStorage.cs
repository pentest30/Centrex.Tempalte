using Saylo.Centrex.Identity.Core.Infrastructure.OpenId.Models;

namespace Saylo.Centrex.Identity.Core.Infrastructure.OpenId;

public interface ICertificateStorage
{
    Task SaveCertificateAsync(string fileName, byte[] certificateData);
    Task<CertificateInfo> LoadCertificateAsync(CertificateMetadata metadata);
    Task DeleteCertificateAsync(string fileName);
}
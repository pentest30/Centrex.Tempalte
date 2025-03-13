using Saylo.Centrex.Identity.Core.Infrastructure.OpenId.Models;

namespace Saylo.Centrex.Identity.Core.Infrastructure.OpenId;

public interface ICertificateGenerator
{
    (CertificateInfo, byte[]) GenerateNewCertificate(int expirationDays);
}
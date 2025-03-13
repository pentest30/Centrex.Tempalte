using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Saylo.Centrex.Identity.Core.Infrastructure.OpenId.Models;

public class CertificateInfo
{
    public X509Certificate2 Certificate { get; set; }
    public RSA?   PublicKeyRSA { get; set; }
    public RSA?   PrivateKeyRSA { get; set; }
    public string Kty { get; set; }
    public string Use { get; set; }
    public string Kid { get; set; }
    public string N { get; set; }
    public string E { get; set; }
}
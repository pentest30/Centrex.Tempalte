using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;
using Saylo.Centrex.Application.Common.Interfaces;
using Saylo.Centrex.Identity.Core.Infrastructure.OpenId.Models;

namespace Saylo.Centrex.Identity.Core.Infrastructure.OpenId;

public class CertificateGenerator : ICertificateGenerator
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private const int KEY_SIZE = 2048;
    private const string CERTIFICATE_NAME = "CN=Saylo.contrex";
    private readonly string _certPassword;

    public CertificateGenerator(
        OpenIdCertificateSettings settings, 
        IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
        _certPassword = settings.CertificateExportPassword;
    }

    public (CertificateInfo, byte[]) GenerateNewCertificate(int expirationDays)
    {
        using var rsa = RSA.Create(KEY_SIZE);
        var request = new CertificateRequest(
            CERTIFICATE_NAME,
            rsa,
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1);

        var certificate = request.CreateSelfSigned(
        _dateTimeProvider.OffsetUtcNow,
        _dateTimeProvider.OffsetUtcNow.AddDays(expirationDays));

        var exportedCert = certificate.Export(X509ContentType.Pfx, _certPassword);
        var certificatePfx = new X509Certificate2(exportedCert, _certPassword);
        var rsaPublicKey = certificate.GetRSAPublicKey();

        var certInfo = new CertificateInfo
        {
            Certificate = certificatePfx,
            PublicKeyRSA = rsaPublicKey,
            PrivateKeyRSA = rsa,
            Kty = "RSA",
            Use = "sig",
            Kid = Guid.NewGuid().ToString(),
            N = Base64UrlEncoder.Encode(rsaPublicKey.ExportParameters(false).Modulus),
            E = Base64UrlEncoder.Encode(rsaPublicKey.ExportParameters(false).Exponent)
        };

        return (certInfo, exportedCert);
    }
}
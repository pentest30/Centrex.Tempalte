using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Saylo.Centrex.Identity.Core.Infrastructure.OpenId.Models;

namespace Saylo.Centrex.Identity.Core.Infrastructure.Services
{
    public interface ICertificateService
    {
        CertificateInfo GetCurrentCertificate();
        CertificateInfo GetPreviousCertificate();
        void RotateCertificates();
    }

    public class CertificateService : ICertificateService
    {
        private CertificateInfo _currentCertificate;
        private CertificateInfo _previousCertificate;
        private readonly TimeSpan _rotationPeriod;
        private DateTime _lastRotation;


        public CertificateService(TimeSpan rotationPeriod)
        {
            _rotationPeriod = rotationPeriod;
            _lastRotation = DateTime.UtcNow;
            _currentCertificate = GenerateCertificate();
        }

        public CertificateInfo GetCurrentCertificate()
        {
            RotateCertificatesIfNeeded();
            return _currentCertificate;
        }

        public CertificateInfo GetPreviousCertificate()
        {
            return _previousCertificate;
        }

        public void RotateCertificates()
        {
            _previousCertificate = _currentCertificate;
            _currentCertificate = GenerateCertificate();
            _lastRotation = DateTime.UtcNow;

        }

        private void RotateCertificatesIfNeeded()
        {
            if (_currentCertificate == null)
            {
                // gen current cert     
                _currentCertificate = GenerateCertificate();
            }
            if (DateTime.UtcNow - _lastRotation > _rotationPeriod)
            {
                RotateCertificates();
            }
        }

        private CertificateInfo GenerateCertificate()
        {
            using (var rsa = RSA.Create(2048))
            {
                var _request = new CertificateRequest("CN=Saylo.contrex", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                var _certificate = _request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddDays(5));
                var _certificatePfx = new X509Certificate2(_certificate.Export(X509ContentType.Pfx));

                var rsaPublicKey = _certificate.GetRSAPublicKey();

                CertificateInfo _certificateInfo = new CertificateInfo
                {
                    Certificate = _certificatePfx,
                    PublicKeyRSA = rsaPublicKey,
                    PrivateKeyRSA = rsa,
                    Kty = "RSA",
                    Use = "sig",
                    Kid = Guid.NewGuid().ToString(),
                    N = Base64UrlEncoder.Encode(rsaPublicKey.ExportParameters(false).Modulus),
                    E = Base64UrlEncoder.Encode(rsaPublicKey.ExportParameters(false).Exponent)
                };
                return _certificateInfo;
            }
        }
    }
}

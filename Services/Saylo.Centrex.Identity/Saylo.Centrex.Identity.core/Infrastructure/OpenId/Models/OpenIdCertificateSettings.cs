namespace Saylo.Centrex.Identity.Core.Infrastructure.OpenId.Models;

public class OpenIdCertificateSettings
{
    public string CertificateStorePath { get; set; }
    public int KeyRotationPeriodDays { get; set; }
    public int CertificateLifeSpanDays { get; set; }
    public string CertificateExportPassword { get; set; }

    public void Validate()
    {
        if (string.IsNullOrEmpty(CertificateStorePath))
            throw new ArgumentNullException(nameof(CertificateStorePath));
            
        if (KeyRotationPeriodDays <= 0)
            throw new ArgumentException("Must be positive", nameof(KeyRotationPeriodDays));
            
        if (CertificateLifeSpanDays <= 0)
            throw new ArgumentException("Must be positive", nameof(CertificateLifeSpanDays));
            
        if (string.IsNullOrEmpty(CertificateExportPassword))
            throw new ArgumentNullException(nameof(CertificateExportPassword));
    }
}
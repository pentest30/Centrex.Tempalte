namespace Saylo.Centrex.Identity.Core.Infrastructure.OpenId.Models;

public class CertificateMetadata
{
    public Guid Id { get; set; }
    public string Filename { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string Thumbprint { get; set; }
    public string Kid { get; set; }
    public bool IsActive { get; set; }
    public KeyParameters KeyParameters { get; set; }
}
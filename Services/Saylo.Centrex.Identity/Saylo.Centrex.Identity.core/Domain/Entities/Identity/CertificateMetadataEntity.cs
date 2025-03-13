using Saylo.Centrex.Domain.Entities;

namespace Saylo.Centrex.Identity.Core.Domain.Entities.Identity;

public class CertificateMetadataEntity : AggregateRoot<Guid>
{
    public string Filename { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string Thumbprint { get; set; }
    public string Kid { get; set; }
    public bool IsActive { get; set; }
    public string Kty { get; set; }
    public string Use { get; set; }
    public string N { get; set; }
    public string E { get; set; }
}
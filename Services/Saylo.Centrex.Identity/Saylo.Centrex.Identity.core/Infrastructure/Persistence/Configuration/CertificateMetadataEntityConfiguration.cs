using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saylo.Centrex.Identity.Core.Domain.Entities.Identity;

namespace Saylo.Centrex.Identity.Core.Infrastructure.Persistence.Configuration;

public class CertificateMetadataEntityConfiguration : IEntityTypeConfiguration<CertificateMetadataEntity>
{
    public void Configure(EntityTypeBuilder<CertificateMetadataEntity> builder)
    {
        builder.ToTable("CertificateMetadata", SchemaNames.Admin);
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.Thumbprint);
        builder.HasIndex(x => x.Kid);
    }
}
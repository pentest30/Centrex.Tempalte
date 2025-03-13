using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saylo.Centrex.Identity.Core.Domain.Entities;

namespace Saylo.Centrex.Identity.Core.Infrastructure.Persistence.Configuration
{
    internal class FunctionalityConfiguration :
        IEntityTypeConfiguration<Functionality>,
        IEntityTypeConfiguration<Module>

    {
        public void Configure(EntityTypeBuilder<Functionality> builder)
        {
            builder.ToTable("Functionalities", SchemaNames.Admin);
            builder.HasMany(f => f.AdministrationDomains)
                .WithMany(e => e.Functionalities)
                .UsingEntity(
                    "AdministrationDomainFunctionalities",
                    l => l.HasOne(typeof(AdministrationDomain)).WithMany().HasForeignKey("AdministrationDomainId"),
                    r => r.HasOne(typeof(Functionality)).WithMany().HasForeignKey("FunctionalityId"),
                    j =>
                    {
                        j.ToTable("AdministrationDomainFunctionalities", SchemaNames.Admin);
                        j.HasKey("AdministrationDomainId", "FunctionalityId");
                    });

        }

        public void Configure(EntityTypeBuilder<Module> builder)
        {
            builder.ToTable("Modules", SchemaNames.Admin);
        }
    }
}

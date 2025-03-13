using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saylo.Centrex.Identity.Core.Domain.Entities;

namespace Saylo.Centrex.Identity.Core.Infrastructure.Persistence.Configuration;

public class AdministrationDomainConfiguration : IEntityTypeConfiguration<AdministrationDomain>
{
    public void Configure(EntityTypeBuilder<AdministrationDomain> builder)
    {
        builder.ToTable("AdministrationDomains", SchemaNames.Admin);
        // Only configure Administrator property
        builder.Property(x => x.Administrator)
            .HasMaxLength(200);
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnType("nvarchar(200)");

        builder.Property(x => x.ConnectionString)
            .HasMaxLength(500)
            .HasColumnType("nvarchar(500)");

        builder.Property(x => x.Domain)
            .HasMaxLength(200)
            .HasColumnType("nvarchar(200)");

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true)
            .HasColumnType("bit");

        builder.Property(x => x.Email)
            .HasMaxLength(256)
            .HasColumnType("nvarchar(256)");

        builder.Property(x => x.Administrator)
            .HasMaxLength(200)
            .HasColumnType("nvarchar(200)");

        // Indexes
        builder.HasIndex(x => x.Name)
            .IsUnique();
        ConfigureSeedData(builder);
    }
    private void ConfigureSeedData(EntityTypeBuilder<AdministrationDomain> builder)
    {
        builder.HasData(new AdministrationDomain
        {
            Id = Guid.Parse("bdcb855f-a6a0-444f-997e-0004603d6c93"),
            Name = "Root tenant",
            ConnectionString = String.Empty,
            Domain = "root.tenant.com",
            IsActive = true,
            Email = "admin@roottenant.com",
            Administrator = "Admin User",
            TypeEntity = TypeEntity.Base
        });
    }
}

public class EnterpriseConfiguration : IEntityTypeConfiguration<Enterprise>
{
    public void Configure(EntityTypeBuilder<Enterprise> builder)
    {
        builder.ToTable("Enterprises", SchemaNames.Admin);
        
        builder.Property(e => e.Siret)
            .HasMaxLength(14)
            .IsRequired();
        builder.Property(e => e.Name)
            .HasMaxLength(140)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();
        
        builder.HasIndex(x => x.Siret)
            .IsUnique();
        builder.OwnsOne(e => e.MainAddress, addressBuilder =>
        {
            addressBuilder.Property(a => a.Street).HasMaxLength(100).IsRequired();
            addressBuilder.Property(a => a.City).HasMaxLength(50).IsRequired();
            addressBuilder.Property(a => a.PostalCode).HasMaxLength(10).IsRequired();
            addressBuilder.Property(a => a.Country).HasMaxLength(50).IsRequired();
        });

        builder.OwnsOne(e => e.SecondAddress, addressBuilder =>
        {
            addressBuilder.Property(a => a.Street).HasMaxLength(100);
            addressBuilder.Property(a => a.City).HasMaxLength(50);
            addressBuilder.Property(a => a.PostalCode).HasMaxLength(10);
            addressBuilder.Property(a => a.Country).HasMaxLength(50);
        });
    }
}

public class ServiceProviderConfiguration : IEntityTypeConfiguration<ServiceProvider>
{
    public void Configure(EntityTypeBuilder<ServiceProvider> builder)
    {
        builder.ToTable("ServiceProviders", SchemaNames.Admin);
        builder.Property(e => e.Name)
            .HasMaxLength(140)
            .IsRequired();

        builder.HasIndex(x => x.Name)
            .IsUnique();

        builder.Property(x => x.ServiceType)
            .HasMaxLength(100);
        
        builder.Property(x => x.ContactEmail)
            .HasMaxLength(100);
    }
}
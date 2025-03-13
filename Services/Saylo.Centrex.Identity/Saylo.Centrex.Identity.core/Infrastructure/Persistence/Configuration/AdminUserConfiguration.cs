using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saylo.Centrex.Identity.Core.Domain.Entities.Identity;

namespace Saylo.Centrex.Identity.Core.Infrastructure.Persistence.Configuration;

internal class AdminUserConfiguration : IEntityTypeConfiguration<ApplicationUser>,
    IEntityTypeConfiguration<AdminRole>,
    IEntityTypeConfiguration<AdminRoleClaim>,
    IEntityTypeConfiguration<AdminUserRole>,
    IEntityTypeConfiguration<AdminUserClaim>,
    IEntityTypeConfiguration<AdminUserToken>,
    IEntityTypeConfiguration<AdminUserLogin>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("IdentityUser", SchemaNames.Admin);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.UserName).HasMaxLength(256);
        builder.Property(x => x.NormalizedUserName).HasMaxLength(256);
        builder.Property(x => x.Email).HasMaxLength(256);
        builder.Property(x => x.NormalizedEmail).HasMaxLength(256);
        builder.Property(x => x.PasswordHash).HasMaxLength(256);
        builder.Property(x => x.ConcurrencyStamp).HasMaxLength(256);
        builder.Property(x => x.PhoneNumber).HasMaxLength(256);
        builder.Property(x => x.SecurityStamp).HasMaxLength(256);
        builder.Property(x => x.TwoFactorEnabled);
        builder.Property(x => x.LockoutEnd);
        builder.Property(x => x.LockoutEnabled);
        builder.Property(x => x.AccessFailedCount);
        builder.Property(x => x.EmailConfirmed);
        builder.Property(x => x.PhoneNumberConfirmed);
        builder.Property(x => x.TypeUser).HasConversion<string>();
        builder.HasOne(u => u.Tenant)
            .WithMany(e => e.Users)
            .HasForeignKey(u => u.TenantId)
            .HasConstraintName("FK_Users_Tenants")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
        // Add filter for active users
        builder.HasQueryFilter(u => u.IsActive);
        
    }

    public void Configure(EntityTypeBuilder<AdminRole> builder)
    {
        builder.ToTable("IdentityRole", SchemaNames.Admin);
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.Name).HasMaxLength(256);
        builder.Property(x => x.NormalizedName).HasMaxLength(256);
        builder.Property(x => x.ConcurrencyStamp).HasMaxLength(256);
    }

    public void Configure(EntityTypeBuilder<AdminRoleClaim> builder)
    {
        builder.ToTable("IdentityRoleClaim", SchemaNames.Admin);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.Property(x => x.ClaimType).HasMaxLength(256);
        builder.Property(x => x.ClaimValue).HasMaxLength(256);
        builder.HasOne(rc => rc.Role)
            .WithMany(r => r.RoleClaims)
            .HasForeignKey(rc => rc.RoleId);
    }

    public void Configure(EntityTypeBuilder<AdminUserRole> builder)
    {
        builder.ToTable("IdentityUserRole", SchemaNames.Admin);

        builder.HasKey(x => new { x.UserId, x.RoleId });


        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);


        builder.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);
    }

    public void Configure(EntityTypeBuilder<AdminUserClaim> builder)
    {
        builder.ToTable("IdentityUserClaim", SchemaNames.Admin);
    }

    public void Configure(EntityTypeBuilder<AdminUserLogin> builder)
    {
        builder.ToTable("IdentityUserLogin", SchemaNames.Admin);
    }

    public void Configure(EntityTypeBuilder<AdminUserToken> builder)
    {
        builder.ToTable("IdentityUserToken", SchemaNames.Admin);
    }

}
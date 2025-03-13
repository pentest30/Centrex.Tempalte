using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Saylo.Centrex.Domain.Entities;
using Saylo.Centrex.Infrastructure.Persistence;

namespace Saylo.Centrex.Identity.Core.Infrastructure.Persistence.Configuration
{
    public class OutboxEventConfiguration : BaseEntityConfiguration<OutboxEvent>
    {
        public OutboxEventConfiguration() : base(SchemaNames.Admin) { }

        protected override void ConfigureEntity(EntityTypeBuilder<OutboxEvent> builder)
        {
            // Primary Key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.EventType)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(e => e.Error)
                .HasMaxLength(256);

            builder.Property(e => e.ObjectId)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(e => e.Payload)
                .IsRequired();

            builder.Property(e => e.Status)
                .IsRequired();

            // Indexes
            builder.HasIndex(e => new { e.ObjectId, e.EventType, e.Status })
                .HasDatabaseName("IX_OutboxEvent_ObjectId_EventType_Status");

            // Additional configuration
            builder.Property(e => e.Id)
                .ValueGeneratedNever();
        }
    }
}
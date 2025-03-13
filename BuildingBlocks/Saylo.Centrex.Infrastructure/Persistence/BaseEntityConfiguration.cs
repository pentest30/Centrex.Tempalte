using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Saylo.Centrex.Infrastructure.Persistence
{
    public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T> where T : class
    {
        private readonly string _schema;

        protected BaseEntityConfiguration(string schema)
        {
            _schema = schema;
        }

        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            if (!string.IsNullOrWhiteSpace(_schema))
            {
                builder.ToTable(typeof(T).Name, _schema);
            }
            else
            {
                builder.ToTable(typeof(T).Name);
            }

            ConfigureEntity(builder);
        }

        protected abstract void ConfigureEntity(EntityTypeBuilder<T> builder);
    }
}
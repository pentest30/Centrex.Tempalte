using System.ComponentModel.DataAnnotations;

namespace Saylo.Centrex.Domain.Entities
{
    public abstract class Entity<TKey> : IHasKey<TKey>, ITrackable
    {
        public TKey Id { get; set; }

        [Timestamp] public byte[]? RowVersion { get; set; }

        public DateTimeOffset CreatedDateTime { get; set; }

        public DateTimeOffset? UpdatedDateTime { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedById { get; set; }
        public string? UpdateBy { get; set; }
        public string? UpdateById { get; set; }
    }
    
}

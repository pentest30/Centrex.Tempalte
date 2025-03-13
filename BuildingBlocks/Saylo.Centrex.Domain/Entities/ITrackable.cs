namespace Saylo.Centrex.Domain.Entities
{
    public interface ITrackable
    {
        byte[]? RowVersion { get; set; }

        DateTimeOffset CreatedDateTime { get; set; }

        DateTimeOffset? UpdatedDateTime { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedById { get; set; }
        public string? UpdateBy { get; set; }
        public string? UpdateById { get; set; }
    }
}

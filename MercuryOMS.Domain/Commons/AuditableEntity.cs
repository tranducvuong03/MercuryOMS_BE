namespace MercuryOMS.Domain.Commons
{
    public abstract class AuditableEntity : BaseEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedAt { get; set; } = DateTime.UtcNow;
    }
}

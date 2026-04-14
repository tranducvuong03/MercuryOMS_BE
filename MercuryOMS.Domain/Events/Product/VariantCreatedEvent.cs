using MercuryOMS.Domain.Commons;

namespace MercuryOMS.Domain.Events
{
    public record VariantCreatedEvent(Guid VariantId, int InitialStock) : IDomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    }
}

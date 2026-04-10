using MercuryOMS.Domain.Commons;

namespace MercuryOMS.Domain.Events
{
    public record PaymentPaidEvent(
        Guid PaymentId,
        Guid OrderId,
        decimal Amount
    ) : IDomainEvent
    {
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;
    }
}

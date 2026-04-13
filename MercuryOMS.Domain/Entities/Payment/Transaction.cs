using MercuryOMS.Domain.Commons;
using MercuryOMS.Domain.Enums;

namespace MercuryOMS.Domain.Entities
{
    public class Transaction : BaseEntity
    {
        public Guid PaymentId { get; private set; }
        public Guid OrderId { get; private set; }

        public decimal Amount { get; private set; }
        public TransactionType Type { get; private set; }
        public DateTime OccurredAt { get; private set; }

        private Transaction() { }

        public Transaction(
            Guid paymentId,
            Guid orderId,
            decimal amount,
            TransactionType type)
        {
            Id = Guid.NewGuid();
            PaymentId = paymentId;
            OrderId = orderId;
            Amount = amount;
            Type = type;
            OccurredAt = DateTime.UtcNow;
        }
    }
}

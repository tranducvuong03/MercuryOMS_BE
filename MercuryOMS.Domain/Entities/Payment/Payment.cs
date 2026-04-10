using MercuryOMS.Domain.Commons;
using MercuryOMS.Domain.Enums;
using MercuryOMS.Domain.Events;
using MercuryOMS.Domain.Exceptions;

namespace MercuryOMS.Domain.Entities
{
    public class Payment : AggregateRoot
    {
        public Guid OrderId { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentStatus Status { get; private set; }
        public string PaymentMethod { get; private set; } = default!;
        public DateTime CreatedAt { get; private set; }

        private Payment() { }

        public Payment(Guid orderId, decimal amount, string paymentMethod)
        {
            if (amount <= 0)
                throw new DomainException("Số tiền phải lớn hơn 0.");

            if (string.IsNullOrWhiteSpace(paymentMethod))
                throw new DomainException("Phương thức thanh toán không hợp lệ.");

            Id = Guid.NewGuid();
            OrderId = orderId;
            Amount = amount;
            PaymentMethod = paymentMethod;
            Status = PaymentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkPaid()
        {
            if (Status != PaymentStatus.Pending)
                throw new DomainException("Thanh toán không ở trạng thái chờ xử lý.");

            Status = PaymentStatus.Paid;

            AddDomainEvent(new PaymentPaidEvent(Id, OrderId, Amount));
        }

        public void MarkFailed(string reason)
        {
            if (Status != PaymentStatus.Pending)
                throw new DomainException("Thanh toán không ở trạng thái chờ xử lý.");

            if (string.IsNullOrWhiteSpace(reason))
                throw new DomainException("Lý do thất bại không hợp lệ.");

            Status = PaymentStatus.Failed;

            // AddDomainEvent(new PaymentFailedEvent(Id, OrderId, reason));
        }
    }
}
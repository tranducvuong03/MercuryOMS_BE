using MercuryOMS.Domain.Commons;
using MercuryOMS.Domain.Exceptions;

namespace MercuryOMS.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; private set; }
        public Guid ProductVariantId { get; private set; }

        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        public decimal Total => Quantity * UnitPrice;

        private OrderItem() { }

        internal OrderItem(
            Guid orderId,
            Guid productVariantId,
            int quantity,
            decimal unitPrice)
        {
            if (quantity <= 0)
                throw new DomainException("Số lượng phải lớn hơn 0.");

            if (unitPrice < 0)
                throw new DomainException("Đơn giá không được âm.");

            Id = Guid.NewGuid();
            OrderId = orderId;
            ProductVariantId = productVariantId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        internal void IncreaseQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Số lượng phải lớn hơn 0.");

            Quantity += quantity;
        }
    }
}
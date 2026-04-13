using MercuryOMS.Domain.Commons;
using MercuryOMS.Domain.Enums;
using MercuryOMS.Domain.Exceptions;

namespace MercuryOMS.Domain.Entities
{
    public class Order : AggregateRoot
    {
        public DateTime OrderDate { get; private set; }
        public Guid UserId { get; private set; }
        public OrderStatus Status { get; private set; }

        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        public decimal TotalAmount => _items.Sum(i => i.Total);

        private Order() { }

        public Order(Guid userId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            OrderDate = DateTime.UtcNow;
            Status = OrderStatus.Pending;
        }

        public void AddItem(Guid variantId, int quantity, decimal unitPrice)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity must be greater than zero.");

            if (unitPrice < 0)
                throw new DomainException("Unit price cannot be negative.");

            var existing = _items.FirstOrDefault(x => x.ProductVariantId == variantId);

            if (existing != null)
            {
                existing.IncreaseQuantity(quantity);
                return;
            }

            var item = new OrderItem(Id, variantId, quantity, unitPrice);
            _items.Add(item);
        }

        public void RemoveItem(Guid variantId)
        {
            var item = _items.FirstOrDefault(x => x.ProductVariantId == variantId);
            if (item != null)
                _items.Remove(item);
        }

        public void MarkAsPaid()
        {
            if (Status == OrderStatus.Paid)
                return;

            if (Status == OrderStatus.Cancelled)
                throw new DomainException("Đơn hàng đã bị huỷ, không thể thanh toán.");

            Status = OrderStatus.Paid;
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Paid)
                throw new DomainException("Đơn hàng đã thanh toán, không thể huỷ.");

            Status = OrderStatus.Cancelled;
        }
    }
}

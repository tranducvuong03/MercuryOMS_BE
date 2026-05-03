using MercuryOMS.Domain.Commons;
using MercuryOMS.Domain.Enums;
using MercuryOMS.Domain.Exceptions;
using MercuryOMS.Domain.ValueObjects;

namespace MercuryOMS.Domain.Entities
{
    public class Order : AggregateRoot
    {
        public DateTime OrderDate { get; private set; }
        public Guid UserId { get; private set; }
        public OrderStatus Status { get; private set; }
        public Address ShippingAddress { get; private set; } = null!;

        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        public decimal TotalAmount => _items.Sum(i => i.Total);

        private Order() { }

        public Order(Guid userId, Address address)
        {
            if (address == null)
                throw new DomainException("Địa chỉ không hợp lệ.");

            UserId = userId;
            OrderDate = DateTime.UtcNow;
            Status = OrderStatus.Pending;
            ShippingAddress = address;
        }

        public void AddItem(Guid productId, Guid variantId, int quantity, decimal unitPrice)
        {
            if (quantity <= 0)
                throw new DomainException("Số lượng phải lớn hơn 0.");

            if (unitPrice < 0)
                throw new DomainException("Đơn giá không thể âm.");

            var existing = _items.FirstOrDefault(x => x.ProductVariantId == variantId);

            if (existing != null)
            {
                existing.IncreaseQuantity(quantity);
                return;
            }

            var item = new OrderItem(Id, productId, variantId, quantity, unitPrice);
            _items.Add(item);
        }

        public void RemoveItem(Guid variantId)
        {
            var item = _items.FirstOrDefault(x => x.ProductVariantId == variantId);
            if (item != null)
                _items.Remove(item);
        }

        public void MarkAsConfirmed()
        {
            if (Status != OrderStatus.Pending)
                throw new DomainException("Chỉ đơn Pending mới được xác nhận.");

            Status = OrderStatus.Confirmed;
        }

        public void MarkAsProcessing()
        {
            if (Status != OrderStatus.Confirmed)
                throw new DomainException("Phải xác nhận trước.");

            Status = OrderStatus.Processing;
        }

        public void MarkAsShipping()
        {
            if (Status != OrderStatus.Processing)
                throw new DomainException("Phải xử lý trước.");

            Status = OrderStatus.Shipping;
        }

        public void MarkAsCompleted()
        {
            if (Status != OrderStatus.Shipping)
                throw new DomainException("Phải đang giao hàng.");

            Status = OrderStatus.Completed;
        }

        public void Cancel()
        {
            if (Status == OrderStatus.Cancelled)
                throw new DomainException("Đơn đã bị huỷ.");

            if (Status != OrderStatus.Pending && Status != OrderStatus.Confirmed)
                throw new DomainException("Chỉ có thể huỷ khi đơn đang ở trạng thái Pending hoặc Confirmed.");

            Status = OrderStatus.Cancelled;
        }

        public Review CreateReview(Guid orderItemId, int rating, string? comment)
        {
            var item = _items.FirstOrDefault(x => x.Id == orderItemId);

            if (item == null)
                throw new DomainException("Item không tồn tại.");

            if (Status != OrderStatus.Completed)
                throw new DomainException("Chỉ được review khi đơn đã hoàn thành.");

            item.MarkAsReviewed();

            return new Review(item.Id, rating, comment);
        }
    }
}
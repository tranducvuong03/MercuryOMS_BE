using MercuryOMS.Domain.Commons;
using MercuryOMS.Domain.Exceptions;

namespace MercuryOMS.Domain.Entities
{
    public class Order : AggregateRoot
    {
        public DateTime OrderDate { get; private set; }
        public Guid UserId { get; private set; }

        private readonly List<OrderItem> _items = new();
        public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

        public decimal TotalAmount => _items.Sum(i => i.Total);

        private Order() { }

        public Order(Guid userId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            OrderDate = DateTime.UtcNow;
        }

        public void AddItem(Guid productId, int quantity, decimal unitPrice)
        {
            if (quantity <= 0)
                throw new DomainException("Quantity must be greater than zero.");

            if (unitPrice < 0)
                throw new DomainException("Unit price cannot be negative.");

            var existing = _items.FirstOrDefault(x => x.ProductId == productId);
            if (existing != null)
            {
                existing.IncreaseQuantity(quantity);
                return;
            }

            var item = new OrderItem(Id, productId, quantity, unitPrice);
            _items.Add(item);
        }

        public void RemoveItem(Guid productId)
        {
            var item = _items.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
                _items.Remove(item);
        }
    }
}

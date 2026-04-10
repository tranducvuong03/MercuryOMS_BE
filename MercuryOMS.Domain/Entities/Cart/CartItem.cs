using MercuryOMS.Domain.Commons;
using MercuryOMS.Domain.Exceptions;

namespace MercuryOMS.Domain.Entities
{
    public class CartItem : BaseEntity
    {
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }

        private CartItem() { }

        internal CartItem(Guid productId, int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Số lượng phải lớn hơn 0.");

            Id = Guid.NewGuid();
            ProductId = productId;
            Quantity = quantity;
        }

        internal void SetQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new DomainException("Số lượng phải lớn hơn 0.");

            Quantity = quantity;
        }

        internal void Increase(int amount)
        {
            if (amount <= 0)
                throw new DomainException("Số lượng tăng phải lớn hơn 0.");

            Quantity += amount;
        }

        internal void Decrease(int amount)
        {
            if (amount <= 0)
                throw new DomainException("Số lượng giảm phải lớn hơn 0.");

            if (Quantity - amount <= 0)
                throw new DomainException("Số lượng không thể nhỏ hơn hoặc bằng 0.");

            Quantity -= amount;
        }
    }
}
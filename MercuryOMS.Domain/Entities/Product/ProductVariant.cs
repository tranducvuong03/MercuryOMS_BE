using MercuryOMS.Domain.Commons;
using MercuryOMS.Domain.Exceptions;

namespace MercuryOMS.Domain.Entities
{
    public class ProductVariant : BaseEntity
    {
        public Guid ProductId { get; private set; }
        public string Sku { get; private set; } = null!;  //mã định danh duy nhất cho biến thể sản phẩm
        public decimal Price { get; private set; }
        public int Stock { get; private set; }
        public string Color { get; private set; }
        public string? Size { get; private set; }

        private ProductVariant() { }

        public ProductVariant(Guid productId, string sku, decimal price, string color, string? size)
        {
            Id = Guid.NewGuid();

            ProductId = productId;
            Sku = sku;
            Price = price;
            Color = color;
            Size = size;
        }

        internal void SetStock(int stock)
        {
            if (stock < 0)
                throw new DomainException("Hàng còn lại không được âm.");

            Stock = stock;
        }
    }
}

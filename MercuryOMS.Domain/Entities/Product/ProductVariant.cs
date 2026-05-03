using MercuryOMS.Domain.Commons;

namespace MercuryOMS.Domain.Entities
{
    public class ProductVariant : BaseEntity
    {
        public Guid ProductId { get; private set; }

        public string Sku { get; private set; } = null!;
        public decimal OriginalPrice { get; private set; } // giá gốc
        public decimal? DiscountPrice { get; private set; }

        public string Color { get; private set; }
        public string? Size { get; private set; }
        public string? ImageUrl { get; private set; }

        private ProductVariant() { }

        public ProductVariant(
            Guid productId,
            string sku,
            decimal originalPrice,
            decimal? discountPrice,
            string color,
            string? size,
            string? imageUrl)
        {
            Id = Guid.NewGuid();

            ProductId = productId;
            Sku = sku;
            OriginalPrice = originalPrice;
            DiscountPrice = discountPrice;

            Color = color;
            Size = size;
            ImageUrl = imageUrl;
        }
    }
}
namespace MercuryOMS.Application.Models.Responses
{
    public class CartItemResponse
    {
        public Guid ProductId { get; set; }
        public Guid VariantId { get; set; }

        public string ProductName { get; set; } = default!;
        public string Image { get; set; } = default!;

        public string Color { get; set; } = default!;
        public string? Size { get; set; }

        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }

        public int Quantity { get; set; }

        public int Stock { get; set; }

        public decimal Total => (DiscountPrice ?? Price) * Quantity;
    }
}

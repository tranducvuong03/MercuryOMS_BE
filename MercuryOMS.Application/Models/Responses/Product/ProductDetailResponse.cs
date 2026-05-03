namespace MercuryOMS.Application.Models.Responses
{
    public class ProductDetailResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public string? Category { get; set; }

        public decimal OriginalPrice { get; set; }
        public decimal? DiscountPrice { get; set; }

        public List<string> Images { get; set; } = new();

        public List<ProductVariantResponse> Variants { get; set; } = new();

        public SellerResponse Seller { get; set; } = new();

        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public int Sold { get; set; }
        public decimal? Discount { get; set; }
    }

    public class ProductVariantResponse
    {
        public Guid Id { get; set; }

        public string? Sku { get; set; }
        public string Color { get; set; } = null!;
        public string? Size { get; set; }

        public decimal OriginalPrice { get; set; }
        public decimal? DiscountPrice { get; set; }
        public string? ImageUrl { get; set; }
        public int Stock { get; set; }
    }

    public class SellerResponse
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Avatar { get; set; }
    }
}
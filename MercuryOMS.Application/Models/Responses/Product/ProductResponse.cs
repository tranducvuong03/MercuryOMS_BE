namespace MercuryOMS.Application.Models
{
    public class ProductResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsActive { get; set; }

        public List<ProductCategoryResponse> Categories { get; set; } = new();
        public List<ProductImageResponse> Images { get; set; } = new();
        public List<ProductVariantResponse> Variants { get; set; } = new();
    }

    public class ProductCategoryResponse
    {
        public Guid CategoryId { get; set; }
    }

    public class ProductImageResponse
    {
        public Guid ProductId { get; set; }
        public string Url { get; set; } = null!;
        public bool IsPrimary { get; set; }
    }

    public class ProductVariantResponse
    {
        public Guid Id { get; set; }
        public string Sku { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}

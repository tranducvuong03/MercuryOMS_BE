namespace MercuryOMS.Application.Models
{
    public class ProductResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal? DiscountPrice { get; set; }
        public bool IsActive { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int Sold { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public string? Badge { get; set; }
        public decimal? Discount { get; set; }
    }

    public class ProductCategoryResponse
    {
        public Guid CategoryId { get; set; }
    }

    public class ProductImageResponse
    {
        public string Url { get; set; } = null!;
        public bool IsPrimary { get; set; }
    }
}

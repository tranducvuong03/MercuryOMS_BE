namespace MercuryOMS.Application.Models.Requests
{
    public class CreateVariantRequest
    {
        public string Sku { get; set; } = null!;
        public decimal Price { get; set; }
        public string Color { get; set; } = null!;
        public string? Size { get; set; }
    }
}

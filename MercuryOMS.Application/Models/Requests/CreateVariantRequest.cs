namespace MercuryOMS.Application.Models.Requests
{
    public class CreateVariantRequest
    {
        public string Sku { get; set; } = null!;
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}

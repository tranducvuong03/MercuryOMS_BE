namespace MercuryOMS.Application.Models.Responses
{
    public class CartResponse
    {
        public List<CartItemResponse> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
    }
}

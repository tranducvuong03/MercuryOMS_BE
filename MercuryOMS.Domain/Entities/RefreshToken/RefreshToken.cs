namespace MercuryOMS.Domain.Entities
{
    public class RefreshToken
    {
        public string Token { get; set; } = default!;
        public DateTime ExpiryDate { get; set; }
        public bool IsRevoked { get; set; }

        public string UserId { get; set; } = default!;
    }
}

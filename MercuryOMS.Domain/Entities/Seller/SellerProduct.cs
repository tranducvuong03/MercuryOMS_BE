using MercuryOMS.Domain.Exceptions;

namespace MercuryOMS.Domain.Entities
{
    public class SellerProduct
    {
        public Guid SellerId { get; private set; }
        public Guid ProductId { get; private set; }

        public decimal CommissionRate { get; private set; }

        private SellerProduct() { } 

        internal SellerProduct(Guid sellerId, Guid productId, decimal commissionRate)
        {
            ValidateRate(commissionRate);

            SellerId = sellerId;
            ProductId = productId;
            CommissionRate = commissionRate;
        }

        internal void UpdateCommission(decimal rate)
        {
            ValidateRate(rate);
            CommissionRate = rate;
        }

        private static void ValidateRate(decimal rate)
        {
            if (rate < 0 || rate > 100)
                throw new DomainException("Tỷ lệ hoa hồng phải nằm trong khoảng từ 0 đến 100.");
        }
    }
}

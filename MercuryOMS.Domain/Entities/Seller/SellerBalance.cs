using MercuryOMS.Domain.Exceptions;

namespace MercuryOMS.Domain.Entities
{
    public class SellerBalance
    {
        public Guid SellerId { get; private set; }
        public decimal Amount { get; private set; }

        private SellerBalance() { }

        internal SellerBalance(Guid sellerId)
        {
            SellerId = sellerId;
            Amount = 0;
        }

        internal void Credit(decimal amount)
        {
            if (amount <= 0)
                throw new DomainException("Số tiền phải lớn hơn 0.");

            Amount += amount;
        }

        internal void Debit(decimal amount)
        {
            if (amount <= 0)
                throw new DomainException("Số tiền phải lớn hơn 0.");

            if (Amount < amount)
                throw new DomainException("Số dư của người bán không đủ.");

            Amount -= amount;
        }
    }
}

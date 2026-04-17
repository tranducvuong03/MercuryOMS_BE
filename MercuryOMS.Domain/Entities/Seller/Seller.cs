using MercuryOMS.Domain.Commons;
using MercuryOMS.Domain.Exceptions;

namespace MercuryOMS.Domain.Entities
{
    public class Seller : AggregateRoot, IAuditableUser
    {
        private readonly List<SellerProduct> _products = new();
        public string UserId { get; private set; }
        public string Name { get; private set; } = null!;
        public bool IsActive { get; private set; }

        public SellerBalance Balance { get; private set; } = null!;

        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }

        public IReadOnlyCollection<SellerProduct> Products => _products.AsReadOnly();

        private Seller() { }

        public Seller(string userId, string name)
        {
            Id = Guid.NewGuid();

            UserId = userId;
            SetName(name);

            IsActive = true;
            Balance = new SellerBalance(Id);
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Tên người bán không được để trống.");

            Name = name.Trim();
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;

        public void AddProduct(Guid productId, decimal commissionRate)
        {
            if (commissionRate < 0 || commissionRate > 1)
                throw new DomainException("Tỷ lệ hoa hồng phải nằm trong khoảng từ 0 đến 1.");

            if (_products.Any(x => x.ProductId == productId))
                return;

            _products.Add(new SellerProduct(Id, productId, commissionRate));
        }

        public void RemoveProduct(Guid productId)
        {
            var sp = _products.FirstOrDefault(x => x.ProductId == productId);
            if (sp != null)
                _products.Remove(sp);
        }

        public void Credit(decimal amount) => Balance.Credit(amount);
        public void Debit(decimal amount) => Balance.Debit(amount);
    }
}

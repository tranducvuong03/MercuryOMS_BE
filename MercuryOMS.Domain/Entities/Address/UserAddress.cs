using MercuryOMS.Domain.Commons;
using MercuryOMS.Domain.ValueObjects;

namespace MercuryOMS.Domain.Entities
{
    public class UserAddress : AggregateRoot
    {
        public Guid UserId { get; private set; }

        public string Label { get; private set; } = null!; // "Nhà", "Công ty"

        public Address Address { get; private set; } = null!;

        public bool IsDefault { get; private set; }

        private UserAddress() { }

        public UserAddress(Guid userId, string label, Address address, bool isDefault = false)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Label = label;
            Address = address;
            IsDefault = isDefault;
        }

        public void SetDefault() => IsDefault = true;
        public void RemoveDefault() => IsDefault = false;

        public void Update(string label, Address address)
        {
            Label = label;
            Address = address;
        }
    }
}
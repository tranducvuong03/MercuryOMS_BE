using MercuryOMS.Domain.Exceptions;

namespace MercuryOMS.Domain.ValueObjects
{
    public class Address
    {
        public string ReceiverName { get; }
        public string Phone { get; }
        public string Street { get; }
        public string District { get; }
        public string City { get; }
        public string Province { get; }

        private Address() { }

        public Address(
            string receiverName,
            string phone,
            string street,
            string district,
            string city,
            string province)
        {
            if (string.IsNullOrWhiteSpace(receiverName))
                throw new DomainException("Tên không hợp lệ");

            if (string.IsNullOrWhiteSpace(phone))
                throw new DomainException("SĐT không hợp lệ");

            ReceiverName = receiverName;
            Phone = phone;
            Street = street;
            District = district;
            City = city;
            Province = province;
        }
    }
}
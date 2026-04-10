using MercuryOMS.Domain.Commons;
using MercuryOMS.Domain.Enums;
using MercuryOMS.Domain.Exceptions;

namespace MercuryOMS.Domain.Entities
{
    public class Shipment : AggregateRoot
    {
        public Guid OrderId { get; private set; }

        public ShipmentStatus Status { get; private set; }

        public string ReceiverName { get; private set; } = null!;
        public string Phone { get; private set; } = null!;
        public string Address { get; private set; } = null!;

        public string? Carrier { get; private set; }          // GHN, GHTK, VNPost...
        public string? TrackingCode { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? ShippedAt { get; private set; }
        public DateTime? DeliveredAt { get; private set; }

        private Shipment() { }

        public Shipment(
            Guid orderId,
            string receiverName,
            string phone,
            string address)
        {
            Id = Guid.NewGuid();
            OrderId = orderId;

            ReceiverName = receiverName;
            Phone = phone;
            Address = address;

            Status = ShipmentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void AssignCarrier(string carrier, string trackingCode)
        {
            if (Status != ShipmentStatus.Pending && Status != ShipmentStatus.Ready)
                throw new DomainException("Không thể gán đơn vị vận chuyển cho trạng thái hiện tại.");

            Carrier = carrier;
            TrackingCode = trackingCode;
            Status = ShipmentStatus.Ready;
        }

        public void MarkShipping()
        {
            if (Status != ShipmentStatus.Ready)
                throw new DomainException("Đơn hàng chưa sẵn sàng để giao.");

            Status = ShipmentStatus.Shipping;
            ShippedAt = DateTime.UtcNow;
        }

        public void MarkDelivered()
        {
            if (Status != ShipmentStatus.Shipping)
                throw new DomainException("Đơn hàng chưa ở trạng thái đang giao.");

            Status = ShipmentStatus.Delivered;
            DeliveredAt = DateTime.UtcNow;
        }

        public void MarkFailed()
        {
            if (Status != ShipmentStatus.Shipping)
                throw new DomainException("Đơn hàng chưa ở trạng thái đang giao.");

            Status = ShipmentStatus.Failed;
        }

        public void Cancel()
        {
            if (Status == ShipmentStatus.Delivered)
                throw new DomainException("Đơn hàng đã giao không thể hủy.");

            Status = ShipmentStatus.Cancelled;
        }
    }
}

using MercuryOMS.Domain.Commons;
using MercuryOMS.Domain.Enums;
using MercuryOMS.Domain.Exceptions;
using MercuryOMS.Domain.ValueObjects;
using System.Net;

namespace MercuryOMS.Domain.Entities
{
    public class Shipment : AggregateRoot
    {
        public Guid OrderId { get; private set; }

        public ShipmentStatus Status { get; private set; }

        public Address ShippingAddress { get; private set; } = null!;

        public string? Carrier { get; private set; }      // GHN, GHTK...
        public string? TrackingCode { get; private set; }

        public DateTime CreatedAt { get; private set; }
        public DateTime? ShippedAt { get; private set; }
        public DateTime? DeliveredAt { get; private set; }

        private Shipment() { }

        public Shipment(Guid orderId, Address address)
        {
            if (orderId == Guid.Empty)
                throw new DomainException("OrderId không hợp lệ.");

            Id = Guid.NewGuid();
            OrderId = orderId;
            ShippingAddress = address;

            Status = ShipmentStatus.Pending;
            CreatedAt = DateTime.UtcNow;
        }

        public void AssignCarrier(string carrier, string trackingCode)
        {
            if (Status != ShipmentStatus.Pending && Status != ShipmentStatus.Ready)
                throw new DomainException("Không thể gán đơn vị vận chuyển.");

            if (string.IsNullOrWhiteSpace(carrier))
                throw new DomainException("Carrier không hợp lệ.");

            if (string.IsNullOrWhiteSpace(trackingCode))
                throw new DomainException("Tracking code không hợp lệ.");

            Carrier = carrier;
            TrackingCode = trackingCode;

            Status = ShipmentStatus.Ready;
        }

        public void MarkShipping()
        {
            if (Status != ShipmentStatus.Ready)
                throw new DomainException("Chưa sẵn sàng giao.");

            Status = ShipmentStatus.Shipping;
            ShippedAt = DateTime.UtcNow;
        }

        public void MarkDelivered()
        {
            if (Status != ShipmentStatus.Shipping)
                throw new DomainException("Chưa ở trạng thái giao.");

            Status = ShipmentStatus.Delivered;
            DeliveredAt = DateTime.UtcNow;
        }

        public void MarkFailed()
        {
            if (Status != ShipmentStatus.Shipping)
                throw new DomainException("Chưa ở trạng thái giao.");

            Status = ShipmentStatus.Failed;
        }

        public void Cancel()
        {
            if (Status == ShipmentStatus.Delivered)
                throw new DomainException("Đã giao không thể hủy.");

            Status = ShipmentStatus.Cancelled;
        }
    }
}
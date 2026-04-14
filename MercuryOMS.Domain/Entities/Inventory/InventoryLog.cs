using MercuryOMS.Domain.Commons;
using MercuryOMS.Domain.Enums;
using MercuryOMS.Domain.Exceptions;

namespace MercuryOMS.Domain.Entities
{
    public class InventoryLog : BaseEntity
    {
        public Guid InventoryId { get; private set; }

        public InventoryLogType Type { get; private set; }
        public int Quantity { get; private set; }

        public Guid? ReferenceId { get; private set; }

        public DateTime CreatedAt { get; private set; }

        private InventoryLog() { }

        internal InventoryLog(
            Guid inventoryId,
            InventoryLogType type,
            int quantity,
            Guid? referenceId = null)
        {
            if (quantity < 0)
                throw new DomainException("Số lượng phải lớn hơn 0.");

            Id = Guid.NewGuid();
            InventoryId = inventoryId;
            Type = type;
            Quantity = quantity;
            ReferenceId = referenceId;
            CreatedAt = DateTime.UtcNow;
        }
    }
}

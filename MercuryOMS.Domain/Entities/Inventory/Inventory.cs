using MercuryOMS.Domain.Commons;
using MercuryOMS.Domain.Enums;

namespace MercuryOMS.Domain.Entities
{
    public class Inventory : AggregateRoot, IAuditableUser
    {
        public Guid ProductId { get; private set; }

        public int Available { get; private set; }
        public int Reserved { get; private set; }

        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }

        private readonly List<InventoryLog> _logs = new();
        public IReadOnlyCollection<InventoryLog> Logs => _logs.AsReadOnly();

        private Inventory() { }

        public Inventory(Guid productId, int initialQuantity)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("ProductId không hợp lệ.");

            if (initialQuantity < 0)
                throw new ArgumentException("Số lượng ban đầu không được âm.");

            Id = Guid.NewGuid();
            ProductId = productId;
            Available = initialQuantity;
            Reserved = 0;

            AddLog(InventoryLogType.StockIn, initialQuantity);
            EnsureValidState();
        }

        public void Reserve(int quantity, Guid? referenceId = null)
        {
            if (quantity <= 0)
                throw new ArgumentException("Số lượng phải lớn hơn 0.");

            if (Available < quantity)
                throw new ArgumentException("Không đủ hàng trong kho.");

            Available -= quantity;
            Reserved += quantity;

            EnsureValidState();
            AddLog(InventoryLogType.Reserve, quantity, referenceId);
        }

        public void Commit(int quantity, Guid? referenceId = null)
        {
            if (quantity <= 0)
                throw new ArgumentException("Số lượng phải lớn hơn 0.");

            if (Reserved < quantity)
                throw new ArgumentException("Số lượng xác nhận vượt quá hàng đã giữ.");

            Reserved -= quantity;

            EnsureValidState();
            AddLog(InventoryLogType.Commit, quantity, referenceId);
        }

        public void Release(int quantity, Guid? referenceId = null)
        {
            if (quantity <= 0)
                throw new ArgumentException("Số lượng phải lớn hơn 0.");

            if (Reserved < quantity)
                throw new ArgumentException("Số lượng hoàn trả vượt quá hàng đã giữ.");

            Reserved -= quantity;
            Available += quantity;

            EnsureValidState();
            AddLog(InventoryLogType.Release, quantity, referenceId);
        }

        public void StockIn(int quantity, Guid? referenceId = null)
        {
            if (quantity <= 0)
                throw new ArgumentException("Số lượng phải lớn hơn 0.");

            Available += quantity;

            EnsureValidState();
            AddLog(InventoryLogType.StockIn, quantity, referenceId);
        }

        public void Adjust(int quantity, Guid? referenceId = null)
        {
            if (quantity == 0)
                throw new ArgumentException("Số lượng điều chỉnh phải khác 0.");

            Available += quantity;

            EnsureValidState();
            AddLog(InventoryLogType.Adjust, Math.Abs(quantity), referenceId);
        }

        private void EnsureValidState()
        {
            if (Available < 0 || Reserved < 0)
                throw new InvalidOperationException("Trạng thái kho không hợp lệ.");
        }

        private void AddLog(InventoryLogType type, int quantity, Guid? referenceId = null)
        {
            _logs.Add(new InventoryLog(Id, type, quantity, referenceId));
        }
    }
}
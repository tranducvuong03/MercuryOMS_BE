using MercuryOMS.Application.IRepository;
using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Entities;

namespace MercuryOMS.Worker.PaymentConsumer
{
    public class DeductInventoryHandler : IPaymentPaidHandler
    {
        private readonly IUnitOfWork _uow;
        private readonly IInventoryRepository _inventoryRepo;

        public DeductInventoryHandler(
            IUnitOfWork uow,
            IInventoryRepository inventoryRepo)
        {
            _uow = uow;
            _inventoryRepo = inventoryRepo;
        }

        public async Task HandleAsync(PaymentPaidMessage message)
        {
            var order = await _uow
                .GetRepository<Order>()
                .GetByIdAsync(message.OrderId);

            if (order == null)
                throw new Exception($"Không tìm thấy đơn hàng: {message.OrderId}");

            foreach (var item in order.Items)
            {
                var inventory = await _inventoryRepo
                    .GetByVariantIdAsync(item.ProductVariantId);

                if (inventory == null)
                    throw new Exception(
                        $"Không tìm thấy tồn kho cho biến thể: {item.ProductVariantId}");

                inventory.Commit(item.Quantity, message.OrderId);
            }

            await _uow.SaveChangesAsync();
        }
    }
}
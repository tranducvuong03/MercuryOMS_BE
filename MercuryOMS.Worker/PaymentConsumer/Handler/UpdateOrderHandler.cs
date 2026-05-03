using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Application.UOW;
using MercuryOMS.Domain.Entities;
using MercuryOMS.Domain.Enums;

namespace MercuryOMS.Worker.PaymentConsumer
{
    public class UpdateOrderHandler : IPaymentPaidHandler
    {
        private readonly IUnitOfWork _uow;

        public UpdateOrderHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task HandleAsync(PaymentPaidMessage message)
        {
            var order = await _uow
                .GetRepository<Order>()
                .GetByIdAsync(message.OrderId);

            if (order == null)
                throw new Exception($"Không tìm thấy đơn hàng: {message.OrderId}");

            order.MarkAsCompleted();

            await _uow.SaveChangesAsync();
        }
    }
}
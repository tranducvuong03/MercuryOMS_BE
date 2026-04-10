using MercuryOMS.Application.IServices;
using MercuryOMS.Domain.Entities;

namespace MercuryOMS.Infrastructure.Services
{
    public class VnPayPaymentService : IPaymentStrategyService
    {
        public string Method => "VNPAY";

        public async Task<Payment> CreatePaymentAsync(
            Guid orderId,
            decimal amount,
            CancellationToken ct)
        {
            await Task.Delay(100, ct);

            var payment = new Payment(orderId, amount, Method);

            return payment;
        }
    }
}

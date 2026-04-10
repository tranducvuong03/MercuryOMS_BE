using MercuryOMS.Application.IServices;
using MercuryOMS.Domain.Entities;

namespace MercuryOMS.Infrastructure.Services
{
    public class CodPaymentService : IPaymentStrategyService
    {
        public string Method => "COD";

        public Task<Payment> CreatePaymentAsync(
            Guid orderId,
            decimal amount,
            CancellationToken ct)
        {
            var payment = new Payment(orderId, amount, Method);

            return Task.FromResult(payment);
        }
    }
}

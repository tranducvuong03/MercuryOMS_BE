using MercuryOMS.Domain.Entities;

namespace MercuryOMS.Application.IServices
{
    public interface IPaymentStrategyService
    {
        string Method { get; }

        Task<Payment> CreatePaymentAsync(
            Guid orderId,
            decimal amount,
            CancellationToken ct);
    }
}

using MercuryOMS.Application.Models.Responses;

namespace MercuryOMS.Worker.PaymentConsumer
{
    public interface IPaymentPaidHandler
    {
        Task HandleAsync(PaymentPaidMessage message);
    }
}

using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Worker.PaymentConsumer;

namespace MercuryOMS.Worker
{
    public class PaymentPaidConsumer
    : RabbitMqConsumerBase<PaymentPaidMessage>
    {
        protected override string QueueName => "payment.paid";

        public PaymentPaidConsumer(IServiceScopeFactory scopeFactory)
            : base(scopeFactory)
        {
        }

        protected override async Task HandleMessageAsync(
            IServiceScope scope,
            PaymentPaidMessage message,
            CancellationToken ct)
        {
            var handlers = scope.ServiceProvider
                .GetServices<IPaymentPaidHandler>();

            foreach (var handler in handlers)
            {
                await handler.HandleAsync(message);
            }
        }
    }
}
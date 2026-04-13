using MercuryOMS.Application.IServices;
using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Domain.Constants;

namespace MercuryOMS.Worker.PaymentConsumer
{
    public class SendEmailHandler : IPaymentPaidHandler
    {
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _config;

        public SendEmailHandler(
            INotificationService notificationService,
            IConfiguration config)
        {
            _notificationService = notificationService;
            _config = config;
        }

        public async Task HandleAsync(PaymentPaidMessage message)
        {
            await _notificationService.SendEmailAsync(
                message.Email,
                "[PAYMENT SUCCESSFUL] - MercuryOMS",
                EmailTemplates.PaymentSuccess(
                    message.FullName,
                    message.OrderId.ToString(),
                    message.Amount.ToString("N0"),
                    $"{_config["App:FrontendUrl"]}/orders/{message.OrderId}"
                )
            );
        }
    }
}

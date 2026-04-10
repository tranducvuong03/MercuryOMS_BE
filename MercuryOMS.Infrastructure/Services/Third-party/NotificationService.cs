using MercuryOMS.Application.IServices;

namespace MercuryOMS.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailService _emailService;

        public NotificationService(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public Task SendEmailAsync(string email, string subject, string content)
        {
            return _emailService.SendAsync(email, subject, content);
        }
    }
}

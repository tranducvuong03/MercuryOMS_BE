namespace MercuryOMS.Application.IServices
{
    public interface INotificationService
    {
        Task SendEmailAsync(string to, string subject, string content);
    }
}

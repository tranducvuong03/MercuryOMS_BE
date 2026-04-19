namespace MercuryOMS.Application.IServices
{
    public interface INotificationHub
    {
        Task SendToUserAsync(string userId, object data);
    }
}

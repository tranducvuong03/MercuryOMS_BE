namespace MercuryOMS.Application.IServices
{
    public interface INotificationRealtimeService
    {
        Task SendToUserAsync(Guid userId, object data);
    }
}

using MercuryOMS.Application.IServices;

namespace MercuryOMS.Infrastructure.Services
{
    public class NotificationRealtimeService : INotificationRealtimeService
    {
        private readonly INotificationHub _hub;

        public NotificationRealtimeService(INotificationHub hub)
        {
            _hub = hub;
        }

        public async Task SendToUserAsync(Guid userId, object data)
        {
            await _hub.SendToUserAsync(userId.ToString(), data);
        }
    }
}
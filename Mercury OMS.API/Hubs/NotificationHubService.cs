using MercuryOMS.Application.IServices;
using Microsoft.AspNetCore.SignalR;

namespace MercuryOMS.API.Hubs
{
    public class NotificationHubService : INotificationHub
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationHubService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendToUserAsync(string userId, object data)
        {
            await _hubContext
                .Clients
                .Group(userId)
                .SendAsync("ReceiveNotification", data);
        }
    }
}

using MercuryOMS.Domain.Commons;

namespace MercuryOMS.Domain.Entities.Notification
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public bool IsRead { get; private set; }
        public string Type { get; private set; }

        public Notification(Guid userId, string title, string message, string type)
        {
            UserId = userId;
            Title = title;
            Message = message;
            Type = type;
            IsRead = false;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}

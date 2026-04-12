namespace MercuryOMS.Domain.Entities
{
    public class OutboxMessage
    {
        public Guid Id { get; private set; }
        public string Type { get; private set; } = default!;
        public string Payload { get; set; } = default!;
        public bool IsProcessed { get; set; }
        public DateTime OccurredOn { get; set; }
        public DateTime? ProcessedOn { get; set; }

        private OutboxMessage() { }

        public OutboxMessage(string type, string payload)
        {
            Id = Guid.NewGuid();
            Type = type;
            Payload = payload;
            IsProcessed = false;
            OccurredOn = DateTime.UtcNow;
        }

        public void MarkAsProcessed()
        {
            IsProcessed = true;
        }
    }
}

using MercuryOMS.Application.IServices;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace MercuryOMS.Infrastructure.Services
{
    public class RabbitMqService : IMessageBus
    {
        private readonly ConnectionFactory _factory;

        public RabbitMqService()
        {
            _factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };
        }

        public Task PublishAsync<T>(string queue, T message)
        {
            using var connection = _factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue, true, false, false);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            channel.BasicPublish("", queue, null, body);

            return Task.CompletedTask;
        }
    }
}

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace MercuryOMS.Worker
{
    public abstract class RabbitMqConsumerBase<TMessage> : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly IConnection _connection;
        private readonly IModel _channel;

        protected abstract string QueueName { get; }

        protected RabbitMqConsumerBase(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;

            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            _channel.BasicQos(0, 1, false);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());

                    var message = JsonSerializer.Deserialize<TMessage>(json)
                                  ?? throw new Exception("Invalid message");

                    using var scope = _scopeFactory.CreateScope();

                    await HandleMessageAsync(scope, message, stoppingToken);

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch
                {
                    _channel.BasicNack(ea.DeliveryTag, false, false);
                }
            };

            _channel.BasicConsume(
                queue: QueueName,
                autoAck: false,
                consumer: consumer
            );

            return Task.CompletedTask;
        }

        protected abstract Task HandleMessageAsync(
            IServiceScope scope,
            TMessage message,
            CancellationToken ct);

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
            base.Dispose();
        }
    }
}

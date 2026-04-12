using MercuryOMS.Application.IServices;
using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Domain.Constants;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace MercuryOMS.Worker
{
    public class PaymentPaidConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;

        private IConnection _connection;
        private IModel _channel;

        private const string QueueName = "payment.paid";

        public PaymentPaidConsumer(
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;

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

            _channel.BasicQos(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false
            );
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);
            if (stoppingToken.IsCancellationRequested)
                return Task.CompletedTask;

            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(ea.Body.ToArray());

                    var message = JsonSerializer.Deserialize<PaymentPaidMessage>(json);

                    if (message == null)
                        throw new Exception("Invalid message");

                    using var scope = _scopeFactory.CreateScope();

                    var notificationService =
                        scope.ServiceProvider.GetRequiredService<INotificationService>();

                    await notificationService.SendEmailAsync(
                        message.Email,
                        "PAYMENT SUCCESSFUL - MercuryOMS",
                        EmailTemplates.PaymentSuccess(
                            message.FullName,
                            message.OrderId.ToString(),
                            message.Amount.ToString("N0"),
                            $"{_configuration["App:FrontendUrl"]}/orders/{message.OrderId}"
                        )
                    );

                    _channel.BasicAck(ea.DeliveryTag, false);
                }
                catch
                {
                    _channel.BasicNack(ea.DeliveryTag, false, requeue: false);
                }
            };

            _channel.BasicConsume(
                queue: QueueName,
                autoAck: false,
                consumer: consumer
            );

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            _channel?.Close();
            _connection?.Close();

            base.Dispose();
        }
    }
}
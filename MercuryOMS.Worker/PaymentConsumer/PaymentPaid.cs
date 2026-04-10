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

        public PaymentPaidConsumer(IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;

            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                queue: "payment.paid",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            _configuration = configuration;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);

                    var message = JsonSerializer.Deserialize<PaymentPaidMessage>(json);

                    if (message == null)
                        throw new Exception("Message deserialize failed");

                    using var scope = _scopeFactory.CreateScope();
                    var notificationService = scope.ServiceProvider
                        .GetRequiredService<INotificationService>();

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

                    _channel.BasicAck(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _channel.BasicNack(ea.DeliveryTag, false, requeue: true);
                }
            };

            _channel.BasicConsume(
                queue: "payment.paid",
                autoAck: false,
                consumer: consumer);

            return Task.CompletedTask;
        }
    }
}

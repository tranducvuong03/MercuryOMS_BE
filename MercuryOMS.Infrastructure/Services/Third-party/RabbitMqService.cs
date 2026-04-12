using MercuryOMS.Application.IServices;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public class RabbitMqService : IMessageBus, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqService()
    {
        var factory = new ConnectionFactory
        {
            HostName = "localhost"
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public Task PublishAsync<T>(string routingKey, T message)
    {
        byte[] body;

        if (message is string str)
        {
            body = Encoding.UTF8.GetBytes(str);
        }
        else
        {
            body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
        }

        _channel.BasicPublish(
            exchange: "",
            routingKey: routingKey,
            basicProperties: null,
            body: body
        );

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Close();
        _connection?.Close();
    }
}
using System.Text;
using RabbitMQ.Client;

namespace Messaging;

public class Producer
{
    private readonly string _hostname;
    private readonly string _exchangeName = "BookNotifications";

    public Producer(string hostName)
    {
        _hostname = hostName;
    }


    public void Send(string message)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _hostname,
            Port = 5672,
            UserName = "ssxaqqgm",
            VirtualHost = "ssxaqqgm",
            Password = "***"

        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(
            exchange: _exchangeName,
            type: ExchangeType.Fanout,
            durable: false,
            autoDelete: false,
            arguments: null);

        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(
            exchange: _exchangeName,
            routingKey: "ignored",
            basicProperties: null,
            body: body);
    }
}

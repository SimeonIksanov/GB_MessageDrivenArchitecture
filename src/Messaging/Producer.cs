using System.Text;
using RabbitMQ.Client;

namespace Messaging;

public class Producer
{
    private readonly string _queueName;
    private readonly string _hostname;


    public Producer(string hostName, string queueName)
    {
        _hostname = hostName;
        _queueName = queueName;
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
            exchange: "direct_exchange",
            type: "direct",
            durable: false,
            autoDelete: false,
            arguments: null);

        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(
            exchange: "direct_exchange",
            routingKey: _queueName,
            basicProperties: null,
            body: body);
    }
}

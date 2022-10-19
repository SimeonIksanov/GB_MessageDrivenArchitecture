using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Messaging;

public class Consumer : IDisposable
{
    private readonly string _queueName;
    private readonly string _hostName;
    private readonly string _exchangeName = "BookNotifications";
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public Consumer(string hostName)
    {
        _hostName = hostName;

        var factory = new ConnectionFactory()
        {
            HostName = _hostName,
            Port = 5672,
            UserName = "ssxaqqgm",
            VirtualHost = "ssxaqqgm",
            Password = "***"

        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void Dispose()
    {
        _connection?.Dispose();
        _channel?.Dispose();
    }

    public void Receive(EventHandler<BasicDeliverEventArgs> receiveCallback)
    {
        _channel.ExchangeDeclare(
            exchange: _exchangeName,
            type: ExchangeType.Fanout);

        var queueName = _channel.QueueDeclare().QueueName;

        _channel.QueueBind(
            queue: queueName,
            exchange: _exchangeName,
            routingKey: "" // ignored when ExchangeType.Fanout
         );

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += receiveCallback;

        _channel.BasicConsume(
            queue: queueName,
            autoAck: true,
            consumer: consumer);
    }
}

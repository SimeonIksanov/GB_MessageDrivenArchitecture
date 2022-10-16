using System.Text;
using Messaging;
using Microsoft.Extensions.Hosting;

namespace Restaurant.Notification;

internal class Worker : BackgroundService
{
    private readonly Consumer _consumer;


    public Worker()
    {
        _consumer = new Consumer("goose-01.rmq2.cloudamqp.com", "BookingNotification");
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _consumer.Receive((sender, args) =>
        {
            var body = args.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(" [x] Received {0}", message);
        });
    }
}

using System.Diagnostics;
using System.Text;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Restaurant.Messages;

namespace Restaurant.Booking;

internal class Worker : BackgroundService
{
    private readonly IBus _bus;

    public Worker(IBus bus, Restaurant restaurant)
    {
        _bus = bus;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(10_000, stoppingToken);
            Console.WriteLine("Привет! Желаете забронировать столик?");

            var dateTime = DateTime.Now;
            await _bus.Publish(
                (IBookingRequest)new BookingRequest(NewId.NextGuid(), NewId.NextGuid(), null, dateTime),
                stoppingToken);
        }
    }
}

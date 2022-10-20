using System.Text;
using MassTransit;
using Messaging;
using Microsoft.Extensions.Hosting;
using Restaurant.Messaging;

namespace Restaurant.Booking;

internal class Worker : BackgroundService
{
    private readonly IBus _bus;
    private readonly Restaurant _restaurant;


    public Worker(IBus bus, Restaurant restaurant)
    {
        _bus = bus;
        _restaurant = restaurant;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(10_000, stoppingToken);
            Console.WriteLine("wanna book table?");
            var result = await _restaurant.BookFreeTableAsync(1);
            await _bus.Publish(new TableBooked(Guid.NewGuid(), Guid.NewGuid(), result), stoppingToken);
        }
    }
}
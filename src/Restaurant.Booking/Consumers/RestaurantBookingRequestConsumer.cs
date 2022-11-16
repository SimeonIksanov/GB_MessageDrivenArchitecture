using System;
using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Messages;

namespace Restaurant.Booking.Consumers;

public class RestaurantBookingRequestConsumer : IConsumer<IBookingRequest>
{
    private readonly Restaurant _restaurant;
    private readonly IdempotencyGuard _guard;
    private readonly ILogger<RestaurantBookingRequestConsumer> _logger;

    public RestaurantBookingRequestConsumer(Restaurant restaurant,
        IdempotencyGuard guard,
        ILogger<RestaurantBookingRequestConsumer> logger)
    {
        _restaurant = restaurant;
        _guard = guard;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IBookingRequest> context)
    {
        //Console.WriteLine("Restaurant.Booking.Consumers => RestaurantBookingRequestConsumer => Consume");

        if (_guard.CheckOrAdd(context.Message.OrderId, context.MessageId.ToString()))
        {
            _logger.LogDebug("second time");
            return;
        }

        _logger.LogInformation($"[OrderId: {context.Message.OrderId}] ");
        var result = await _restaurant.BookFreeTableAsync(1);

        await context.Publish<ITableBooked>(
            new TableBooked(
                context.Message.OrderId,
                context.Message.ClientId,
                result
            )
        );
    }
}


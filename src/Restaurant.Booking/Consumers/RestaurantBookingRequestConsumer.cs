using System;
using MassTransit;
using Restaurant.Messages;

namespace Restaurant.Booking.Consumers;

public class RestaurantBookingRequestConsumer : IConsumer<IBookingRequest>
{
    private readonly Restaurant _restaurant;
    private readonly IdempotencyGuard _guard;

    public RestaurantBookingRequestConsumer(Restaurant restaurant,
        IdempotencyGuard guard)
    {
        _restaurant = restaurant;
        _guard = guard;
    }

    public async Task Consume(ConsumeContext<IBookingRequest> context)
    {
        //Console.WriteLine("Restaurant.Booking.Consumers => RestaurantBookingRequestConsumer => Consume");

        if (_guard.CheckOrAdd(context.Message.OrderId, context.MessageId.ToString()))
        {
            Console.WriteLine("second time");
            return;
        }

        Console.Write($"[OrderId: {context.Message.OrderId}] ");
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


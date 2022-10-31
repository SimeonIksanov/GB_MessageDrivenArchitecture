using System;
using MassTransit;
using Restaurant.Messages;

namespace Restaurant.Booking.Consumers;

public class RestaurantBookingRequestConsumer : IConsumer<IBookingRequest>
{
    private readonly Restaurant _restaurant;

    public RestaurantBookingRequestConsumer(Restaurant restaurant)
    {
        _restaurant = restaurant;
    }

    public async Task Consume(ConsumeContext<IBookingRequest> context)
    {
        //Console.WriteLine("Restaurant.Booking.Consumers => RestaurantBookingRequestConsumer => Consume");
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


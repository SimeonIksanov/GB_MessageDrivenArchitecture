using System;
using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Booking.Consumers;

public class BookingKitchenReadyConsumer : IConsumer<IKitchenReady>
{
    private readonly Restaurant _restaurant;

    public BookingKitchenReadyConsumer(Restaurant restaurant)
    {
        _restaurant = restaurant;
    }

    public Task Consume(ConsumeContext<IKitchenReady> context)
    {
        if (!context.Message.Ready)
        {
            _restaurant.CancelBooking(context.Message.OrderId);
        }

        return context.ConsumeCompleted;
    }
}

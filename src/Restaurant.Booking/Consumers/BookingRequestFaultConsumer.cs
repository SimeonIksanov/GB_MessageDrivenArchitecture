using System;
using System.Threading.Tasks;
using MassTransit;
using Restaurant.Messages;

namespace Restaurant.Booking.Consumers;

public class BookingRequestFaultConsumer : IConsumer<Fault<IBookingRequest>>
{
    public Task Consume(ConsumeContext<Fault<IBookingRequest>> context)
    {
        Console.WriteLine("Restaurant.Booking.Consumers => BookingRequestFaultConsumer => Consume");
        Console.WriteLine($"[OrderId {context.Message.Message.OrderId}] Отмена в зале");
        return Task.CompletedTask;
    }
}
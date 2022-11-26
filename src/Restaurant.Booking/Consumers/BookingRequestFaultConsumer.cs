using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Messages;

namespace Restaurant.Booking.Consumers;

public class BookingRequestFaultConsumer : IConsumer<Fault<IBookingRequest>>
{
    private readonly ILogger<BookingRequestFaultConsumer> _logger;

    public BookingRequestFaultConsumer(ILogger<BookingRequestFaultConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<Fault<IBookingRequest>> context)
    {
        _logger.LogDebug("Restaurant.Booking.Consumers => BookingRequestFaultConsumer => Consume");
        _logger.LogInformation($"[OrderId {context.Message.Message.OrderId}] Отмена в зале");
        return Task.CompletedTask;
    }
}
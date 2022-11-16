using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Messages;

namespace Restaurant.Booking.Consumers;

public class BookFailureConsumer : IConsumer<Fault<IBookFailure>>
{
    private readonly ILogger<BookFailureConsumer> _logger;

    public BookFailureConsumer(ILogger<BookFailureConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<Fault<IBookFailure>> context)
    {
        _logger.LogInformation("не удалось забронировать");
        return context.ConsumeCompleted;
    }
}

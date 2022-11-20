using MassTransit;
using Restaurant.Messages;

namespace Restaurant.Booking.Consumers;

public class BookFailureConsumer : IConsumer<Fault<IBookFailure>>
{
    public Task Consume(ConsumeContext<Fault<IBookFailure>> context)
    {
        Console.WriteLine("не удалось забронировать");
        return context.ConsumeCompleted;
    }
}

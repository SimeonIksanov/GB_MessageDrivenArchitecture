using System;
using MassTransit;
using Restaurant.Messages;

namespace Restaurant.Notification.Consumers;

public class NotifyConsumer : IConsumer<INotify>
{
    private readonly Notifier _notifier;

    public NotifyConsumer(Notifier notifier)
    {
        _notifier = notifier;
    }

    public Task Consume(ConsumeContext<INotify> context)
    {
        Console.WriteLine("Restaurant.Notification.Consumers => NotifyConsumer => Consume");
        _notifier.Notify(
            context.Message.OrderId,
            context.Message.ClientId,
            context.Message.Message);

        return Task.CompletedTask;
    }
}


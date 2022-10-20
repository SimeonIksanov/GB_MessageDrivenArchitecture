using System;
using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Notification.Consumers;

public class KitchenReadyConsumer : IConsumer<IKitchenReady>
{
    private readonly Notifier _notifier;

    public KitchenReadyConsumer(Notifier notifier)
    {
        _notifier = notifier;
    }

    public Task Consume(ConsumeContext<IKitchenReady> context)
    {
        var result = context.Message.Ready;

        _notifier.Accept(context.Message.OrderId,
            result ? Accepted.Kitchen : Accepted.Rejected);

        return context.ConsumeCompleted;
    }
}

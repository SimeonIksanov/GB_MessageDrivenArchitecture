using System;
using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Notification.Consumers;

public class NotifierKitchenReadyConsumer : IConsumer<IKitchenReady>
{
    private readonly Notifier _notifier;

    public NotifierKitchenReadyConsumer(Notifier notifier)
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

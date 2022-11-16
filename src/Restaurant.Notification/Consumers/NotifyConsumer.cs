using System;
using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Messages;

namespace Restaurant.Notification.Consumers;

public class NotifyConsumer : IConsumer<INotify>
{
    private readonly Notifier _notifier;
    private readonly ILogger<NotifyConsumer> _logger;

    public NotifyConsumer(Notifier notifier, ILogger<NotifyConsumer> logger)
    {
        _notifier = notifier;
        _logger = logger;
    }

    public Task Consume(ConsumeContext<INotify> context)
    {
        _logger.LogDebug("Restaurant.Notification.Consumers => NotifyConsumer => Consume");
        _notifier.Notify(
            context.Message.OrderId,
            context.Message.ClientId,
            context.Message.Message);

        return Task.CompletedTask;
    }
}


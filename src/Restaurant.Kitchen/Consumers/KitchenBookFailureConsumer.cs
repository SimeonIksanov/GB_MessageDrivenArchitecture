using System;
using MassTransit;
using Restaurant.Messages;

namespace Restaurant.Kitchen.Consumers;

internal class KitchenBookFailureConsumer : IConsumer<IBookFailure>
{
    private readonly Manager _manager;

    public KitchenBookFailureConsumer(Manager manager)
    {
        _manager = manager;
    }

    public Task Consume(ConsumeContext<IBookFailure> context)
    {
        _manager.CancelKitchedOrder(context.Message.OrderId);
        return context.ConsumeCompleted;
    }
}


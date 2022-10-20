using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Kitchen.Consumers;

internal class KitchenTableBookedConsumer : IConsumer<ITableBooked>
{
    private readonly Manager _manager;

    public KitchenTableBookedConsumer(Manager manager)
    {
        _manager = manager;
    }

    public Task Consume(ConsumeContext<ITableBooked> context)
    {
        _manager.CheckKitchenReady(context.Message.OrderId, context.Message.PreOrder);
        return context.ConsumeCompleted;
    }
}

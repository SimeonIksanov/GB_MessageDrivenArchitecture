using MassTransit;
using Restaurant.Messaging;

namespace Restaurant.Kitchen;

internal class Manager
{
    private readonly IBus _bus;

    public Manager(IBus bus)
    {
        _bus = bus;
    }

    public void CheckKitchenReady(Guid orderId, Dish? dish)
    {
        bool kitchenReady = dish is not Dish.Coffee;
        _bus.Publish<IKitchenReady>(new KitchenReady(orderId, kitchenReady));
    }
}

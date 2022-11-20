using MassTransit;
using Restaurant.Messages;

namespace Restaurant.Kitchen;

public class Manager
{
    //private readonly IBus _bus;

    //public Manager(IBus bus)
    //{
    //    _bus = bus;
    //}

    public bool CheckKitchenReady(Guid orderId, Dish? dish)
    {
        //bool kitchenReady = dish is not Dish.Coffee;
        //Console.WriteLine($"Подтверждаю готовность кухни по заказу {orderId}, {kitchenReady}");
        //_bus.Publish<IKitchenReady>(new KitchenReady(orderId, kitchenReady));
        return true;
    }

    public void CancelKitchedOrder(Guid orderId)
    {

    }
}

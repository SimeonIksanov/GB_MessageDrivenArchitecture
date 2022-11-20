using MassTransit;
using Restaurant.Messages;

namespace Restaurant.Kitchen.Consumers;

internal class KitchenBookingRequestedConsumer : IConsumer<IBookingRequest>
{
    private readonly Manager _manager;
    private readonly IdempotencyGuard _guard;

    public KitchenBookingRequestedConsumer(Manager manager,
        IdempotencyGuard guard)
    {
        _manager = manager;
        _guard = guard;
    }

    public async Task Consume(ConsumeContext<IBookingRequest> context)
    {
        if (_guard.CheckOrAdd(context.Message.OrderId, context.MessageId.ToString()))
        {
            Console.WriteLine("second time");
            return;
        }

        if (context.Message.PreOrder == Dish.Lasagna)
        {
            Console.WriteLine(DateTime.Now.ToShortTimeString());
            throw new Exception("Лазанья в заказе!");
        }

        var rnd = Random.Shared.Next(1000, 10_000);
        Console.WriteLine($"[OrderId: {context.Message.OrderId}] Проверка на кухне займет {rnd / 1000.0} секунд");
        await Task.Delay(rnd);

        if (_manager.CheckKitchenReady(context.Message.OrderId, context.Message.PreOrder))
            await context.Publish<IKitchenReady>(new KitchenReady(context.Message.OrderId, true));

        return;
    }
}

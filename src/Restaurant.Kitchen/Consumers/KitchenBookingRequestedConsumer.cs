﻿using MassTransit;
using Restaurant.Messages;

namespace Restaurant.Kitchen.Consumers;

internal class KitchenBookingRequestedConsumer : IConsumer<IBookingRequest>
{
    private readonly Manager _manager;

    public KitchenBookingRequestedConsumer(Manager manager)
    {
        _manager = manager;
    }

    public async Task Consume(ConsumeContext<IBookingRequest> context)
    {
        if (Random.Shared.Next(1_000, 10_000) > 8_000)
        {
            Console.WriteLine(DateTime.Now.ToShortTimeString());
            throw new Exception("Случилась какая-то беда на кухне!");
        }

        var rnd = Random.Shared.Next(1000, 10_000);
        Console.WriteLine($"[OrderId: {context.Message.OrderId}] Проверка на кухне займет {rnd / 1000.0} секунд");
        await Task.Delay(rnd);

        if (_manager.CheckKitchenReady(context.Message.OrderId, context.Message.PreOrder))
            await context.Publish<IKitchenReady>(new KitchenReady(context.Message.OrderId, true));

        return;
    }
}
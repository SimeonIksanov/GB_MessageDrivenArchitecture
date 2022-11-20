using MassTransit;
using Microsoft.Extensions.Logging;
using Restaurant.Messages;

namespace Restaurant.Kitchen.Consumers;

public class KitchenBookingRequestedConsumer : IConsumer<IBookingRequest>
{
    private readonly Manager _manager;
    private readonly IdempotencyGuard _guard;
    private readonly ILogger<KitchenBookingRequestedConsumer> _logger;

    public KitchenBookingRequestedConsumer(Manager manager,
        IdempotencyGuard guard,
        ILogger<KitchenBookingRequestedConsumer> logger)
    {
        _manager = manager;
        _guard = guard;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<IBookingRequest> context)
    {
        if (_guard.CheckOrAdd(context.Message.OrderId, context.MessageId.ToString()))
        {
            _logger.LogDebug("second time");
            return;
        }

        if (context.Message.PreOrder == Dish.Lasagna)
        {
            _logger.LogInformation(DateTime.Now.ToShortTimeString());
            throw new Exception("Лазанья в заказе!");
        }

        var rnd = Random.Shared.Next(1000, 10_000);
        _logger.LogInformation($"[OrderId: {context.Message.OrderId}] Проверка на кухне займет {rnd / 1000.0} секунд");
        await Task.Delay(rnd);

        if (_manager.CheckKitchenReady(context.Message.OrderId, context.Message.PreOrder))
            await context.Publish<IKitchenReady>(new KitchenReady(context.Message.OrderId, true));

        return;
    }
}

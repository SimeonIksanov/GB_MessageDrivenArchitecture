using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Restaurant.Booking.Consumers;
using Restaurant.Booking;
using Restaurant.Messages;
using Restaurant.Kitchen;
using MassTransit.Testing;
using Restaurant.Kitchen.Consumers;
using Restaurant.Notification.Consumers;
using Restaurant.Notification;

namespace CustomerTests;

[TestFixture]
public class IdempotencyTest
{
    private ServiceProvider _provider;
    private ITestHarness _harness;

    [OneTimeSetUp]
    public async Task Init()
    {
        _provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<KitchenBookingRequestedConsumer>(c => c.UseInMemoryOutbox());
                cfg.AddSagaStateMachine<RestaurantBookingSaga, RestaurantBooking>()
                    .InMemoryRepository();
            })
            .AddLogging()
            .AddTransient<Restaurant.Booking.Restaurant>()
            .AddSingleton<IModelRepository<RequestModel>, InMemoryRepository<RequestModel>>()
            .AddTransient<IdempotencyGuard>()
            .AddTransient<Manager>()
            .BuildServiceProvider(true);
        _harness = _provider.GetTestHarness();

        await _harness.Start();
    }

    [OneTimeTearDown]
    public async Task TearDown()
    {
        //await _harness.OutputTimeline(TestContext.Out, options => options.Now().IncludeAddress());
        await _harness.Stop();
        await _provider.DisposeAsync();
    }

    [Test]
    public async Task Kitchen_Booking_request_consumer_is_immutable()
    {
        var orderId = NewId.NextGuid();
        var messageId = NewId.NextGuid();
        var consumer = _harness.GetConsumerHarness<KitchenBookingRequestedConsumer>();

        int publishCount = 10;
        for (int i = 0; i < publishCount; i++)
        {
            await _harness.Bus.Publish((IBookingRequest)new BookingRequest(orderId, orderId, Dish.Coffee, DateTime.Now),
                                       c => { c.MessageId = messageId; });

            var publishedRequestCount = _harness.Published.Select<IBookingRequest>().Count();
            var consumedCount = consumer
                .Consumed
                .Select<IBookingRequest>()
                .Where(x => x.Context.Message.OrderId == orderId)
                .Count();
            Assert.That(consumedCount, Is.EqualTo(1));
        }

        var publishedResponse = _harness.Published
            .Select<IKitchenReady>()
            .Where(x => x.Context.Message.OrderId == orderId);

        var publishedResponseCount = publishedResponse.Count();
        Assert.That(publishedResponseCount, Is.EqualTo(1));
    }
}


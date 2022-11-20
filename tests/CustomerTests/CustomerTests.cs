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
public class CustomerTests
{
    private ServiceProvider _provider;
    private ITestHarness _harness;

    [OneTimeSetUp]
    public async Task Init()
    {
        _provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddConsumer<RestaurantBookingRequestConsumer>(c => c.UseInMemoryOutbox());
                cfg.AddConsumer<KitchenBookingRequestedConsumer>(c => c.UseInMemoryOutbox());
                cfg.AddConsumer<NotifyConsumer>(c => c.UseInMemoryOutbox());
                cfg.AddSagaStateMachine<RestaurantBookingSaga, RestaurantBooking>()
                    .InMemoryRepository();
            })
            .AddLogging()
            .AddTransient<Restaurant.Booking.Restaurant>()
            .AddSingleton<IModelRepository<RequestModel>, InMemoryRepository<RequestModel>>()
            .AddTransient<IdempotencyGuard>()
            .AddTransient<Manager>()
            .AddTransient<Notifier>()
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
    public async Task Any_booking_request_consumed()
    {
        await _harness.Bus.Publish(
            new BookingRequest(Guid.NewGuid(), Guid.NewGuid(), Dish.Coffee, DateTime.Now));

        Assert.That(await _harness.Consumed.Any());

        //await _harness.OutputTimeline(TestContext.Out, options => options.Now().IncludeAddress());
    }

    [Test]
    public async Task Booking_request_consumer_published_table_booked_message()
    {
        var consumer = _provider.GetRequiredService<IConsumerTestHarness<RestaurantBookingRequestConsumer>>();

        var orderId = NewId.NextGuid();
        var bus = _provider.GetRequiredService<IBus>();

        await bus.Publish<IBookingRequest>(
            new BookingRequest(orderId, orderId, Dish.Coffee, DateTime.Now));

        Assert.That(consumer.Consumed
            .Select<IBookingRequest>()
            .Any(x => x.Context.Message.OrderId == orderId), Is.True);

        Assert.That(_harness.Published
            .Select<ITableBooked>()
            .Any(x => x.Context.Message.OrderId == orderId), Is.True);

        //await _harness.OutputTimeline(TestContext.Out, options => options.Now().IncludeAddress());
    }

    [Test]
    public async Task Should_be_easy()
    {
        var orderId = NewId.NextGuid();
        var clientId = NewId.NextGuid();

        await PublishBookingRequestMessage(orderId, clientId);

        Assert.That(await _harness.Consumed.Any<IBookingRequest>());

        var sagaHarness = _provider
            .GetRequiredService<ISagaStateMachineTestHarness<RestaurantBookingSaga, RestaurantBooking>>();

        Assert.That(await sagaHarness
            .Consumed
            .Any<IBookingRequest>());

        Assert.That(await sagaHarness
            .Created
            .Any(x => x.CorrelationId == orderId));

        var saga = sagaHarness.Created.Contains(orderId);

        Assert.That(saga, Is.Not.Null);
        Assert.That(saga.ClientId, Is.EqualTo(clientId));
        Assert.That(await _harness.Published.Any<ITableBooked>());
        Assert.That(await _harness.Published.Any<IKitchenReady>());
        Assert.That(await _harness.Published.Any<INotify>());

        //await _harness.OutputTimeline(TestContext.Out, options => options.Now().IncludeAddress());
    }

    [Test]
    public async Task Booking_request_consumer_published_kitchen_ready_message()
    {
        var orderId = NewId.NextGuid();
        var clientId = NewId.NextGuid();
        await PublishBookingRequestMessage(orderId, clientId);

        var consumer = _harness.GetConsumerHarness<KitchenBookingRequestedConsumer>();

        Assert.That(consumer
            .Consumed
            .Select<IBookingRequest>()
            .Any(x => x.Context.Message.OrderId == orderId), Is.True);

        Assert.That(_harness.Published
            .Select<IKitchenReady>()
            .Any(x => x.Context.Message.OrderId == orderId), Is.True);
    }

    [Test]
    public async Task NotificationMessageConsumed()
    {
        var orderId = NewId.NextGuid();
        var clientId = NewId.NextGuid();

        await _harness.Bus.Publish(new Notify(orderId, clientId, "test message"));
        Assert.That(await _harness.Published.Any<INotify>());

        var consumer = _harness.GetConsumerHarness<NotifyConsumer>();

        Assert.That(consumer
            .Consumed
            .Select<INotify>()
            .Any(x => x.Context.Message.OrderId == orderId), Is.True);
    }

    private async Task PublishBookingRequestMessage(Guid orderId, Guid clientId)
    {
        await _harness.Bus.Publish(new BookingRequest(orderId, clientId, Dish.Coffee, DateTime.Now));
        Assert.That(await _harness.Published.Any<IBookingRequest>());
    }
}


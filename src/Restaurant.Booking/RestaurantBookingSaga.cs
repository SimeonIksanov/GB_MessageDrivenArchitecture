using Automatonymous;
using MassTransit;
using Restaurant.Booking.Consumers;
using Restaurant.Messages;

namespace Restaurant.Booking;

/// <summary>
/// Saga itself
/// </summary>
public sealed class RestaurantBookingSaga : MassTransitStateMachine<RestaurantBooking>
{
    public RestaurantBookingSaga()
    {
        InstanceState(instanceStateProperty: x => x.CurrentState);

        Event(() => BookingRequested,
            x => x.CorrelateById(context => context.Message.OrderId)
                  .SelectId(context => context.Message.OrderId));

        Event(() => TableBooked,
                x => x.CorrelateById(context => context.Message.OrderId));

        Event(() => KitchenReady,
            x => x.CorrelateById(context => context.Message.OrderId));

        CompositeEvent(() => BookingApproved,
            x => x.ReadyEventStatus, KitchenReady, TableBooked);

        Event(() => BookingRequestFault,
            x => x.CorrelateById(m => m.Message.Message.OrderId));

        Event(() => GuestArrived,
            x => x.CorrelateById(m => m.Message.OrderId));

        Schedule(() => BookingExpired,
            tokenIdExpression: x => x.ExpirationId,
            configureSchedule: x =>
            {
                x.Delay = TimeSpan.FromSeconds(7);//was 5
                x.Received = e => e.CorrelateById(context => context.Message.OrderId);
            });

        Schedule(() => ArrivalExpired,
            tokenIdExpression: x => x.ArrivalExpirationId,
            configureSchedule: x =>
            {
                x.Delay = TimeSpan.FromSeconds(15);
                x.Received = e => e.CorrelateById(context => context.Message.OrderId);
            });

        Initially(
            When(BookingRequested)
                .Then(context =>
                {
                    context.Saga.CorrelationId = context.Message.OrderId;
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.ClientId = context.Message.ClientId;
                    context.Saga.ArrivalDelay = context.Message.ArrivalDelay;
                })
                .Schedule(BookingExpired,
                    context => new BookingExpire(context.Saga)//,
                    //context => TimeSpan.FromSeconds(5)
                 )
                .TransitionTo(AwaitingBookingApproved)
        );

        During(AwaitingBookingApproved,
            When(BookingApproved)
                .Unschedule(BookingExpired)
                .Publish(context =>
                    (INotify)new Notify(
                        context.Saga.OrderId,
                        context.Saga.ClientId,
                        $"Стол успешно забронирован"))
                .Schedule(ArrivalExpired, c => new ArrivalExpire(c.Saga))
                .TransitionTo(AwaitingGuestArrival),

            When(BookingExpired.Received)
                .Then(context => Console.WriteLine($"[OrderId: {context.Saga.OrderId}] Бронь не подтверждена. Отмена заказа"))
                .Finalize(),

            When(BookingRequestFault)
                .Then(context => Console.WriteLine($"Ошибочка вышла!"))
                .Publish(context => (INotify)new Notify(
                    context.Saga.OrderId,
                    context.Saga.ClientId,
                    $"Приносим извинения, стол забронировать не получилось."))
                .Finalize()
        );

        During(AwaitingGuestArrival,
            When(GuestArrived)
                .Unschedule(ArrivalExpired)
                .Finalize(),

            When(ArrivalExpired.Received)
                .Then(context => Console.WriteLine($"[OrderId: {context.Saga.OrderId}] Клиент не пришел, отмена"))
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }

    public State AwaitingBookingApproved { get; private set; }
    public Event<IBookingRequest> BookingRequested { get; private set; }
    public Event<ITableBooked> TableBooked { get; private set; }
    public Event<IKitchenReady> KitchenReady { get; private set; }

    public Event<Fault<IBookingRequest>> BookingRequestFault { get; private set; }

    public Schedule<RestaurantBooking, IBookingExpire> BookingExpired { get; private set; }
    public Event BookingApproved { get; private set; }

    public State AwaitingGuestArrival { get; set; }
    public Event<IGuestArrived> GuestArrived { get; set; }
    public Schedule<RestaurantBooking, IArrivalExpire> ArrivalExpired { get; set; }
}

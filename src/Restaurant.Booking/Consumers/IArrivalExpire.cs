using System;
namespace Restaurant.Booking.Consumers;

public interface IArrivalExpire
{
    Guid OrderId { get; }
}

public class ArrivalExpire : IArrivalExpire
{
    private readonly RestaurantBooking _instance;

    public ArrivalExpire(RestaurantBooking instance)
    {
        _instance = instance;
    }

    public Guid OrderId => _instance.OrderId;
}
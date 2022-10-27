namespace Restaurant.Notification;

[Flags]
public enum Accepted
{
    Rejected = 1,
    Kitchen = 2,
    Booking = 4,
    All = Kitchen | Booking
}

namespace Restaurant.Messages;

public class BookingRequest : IBookingRequest
{
    public BookingRequest(Guid orderId, Guid clientId, Dish? preOrder, DateTime creationDate)
    {
        OrderId = orderId;
        ClientId = clientId;
        PreOrder = preOrder;
        CreationDate = creationDate;
        ArrivalDelay = (byte)Random.Shared.Next(7, 15);
    }

    public Guid OrderId { get; }
    public Guid ClientId { get; }
    public Dish? PreOrder { get; }
    public byte ArrivalDelay { get; }
    public DateTime CreationDate { get; }
}

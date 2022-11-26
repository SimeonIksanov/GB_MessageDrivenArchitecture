namespace Restaurant.Messages;

public class GuestArrived : IGuestArrived
{
    public GuestArrived(Guid orderId)
    {
        OrderId = orderId;
    }

    public Guid OrderId { get; set; }
}

using System;
namespace Restaurant.Messages;

public interface IGuestArrived
{
    Guid OrderId { get; }
}

public class GuestArrived : IGuestArrived
{
    public GuestArrived(Guid orderId)
    {
        OrderId = orderId;
    }

    public Guid OrderId { get; set; }
}
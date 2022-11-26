namespace Restaurant.Messages;

public class BookFailure : IBookFailure
{
    public BookFailure(Guid orderId)
    {
        OrderId = orderId;
    }

    public Guid OrderId { get; }
}

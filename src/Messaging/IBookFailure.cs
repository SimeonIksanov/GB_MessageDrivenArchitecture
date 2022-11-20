using System;
namespace Restaurant.Messages;

public interface IBookFailure
{
    Guid OrderId { get; }
}

public class BookFailure : IBookFailure
{
    public BookFailure(Guid orderId)
    {
        OrderId = orderId;
    }

    public Guid OrderId { get; }
}
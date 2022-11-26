using System;
namespace Restaurant.Messages;

public interface IBookFailure
{
    Guid OrderId { get; }
}

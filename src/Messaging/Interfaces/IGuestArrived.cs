using System;
namespace Restaurant.Messages;

public interface IGuestArrived
{
    Guid OrderId { get; }
}

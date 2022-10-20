using System.Collections.Concurrent;

namespace Restaurant.Notification;

public class Notifier
{
    private readonly ConcurrentDictionary<Guid, Tuple<Guid?, Accepted>> _state = new();

    public void Accept(Guid orderId, Accepted accepted, Guid? clientId = null)
    {
        _state.AddOrUpdate(
            key: orderId,
            addValue: new Tuple<Guid?, Accepted>(clientId, accepted),
            updateValueFactory: (guid, oldValue) => new Tuple<Guid?, Accepted>(
                oldValue.Item1 ?? clientId, oldValue.Item2 | accepted
                )
            );
        Notify(orderId);
    }

    private void Notify(Guid orderId)
    {
        var booking = _state[orderId];
        if (booking.Item2.HasFlag(Accepted.All))
        {
            Console.WriteLine($"Successfully booked for client {booking.Item1}");
            _state.Remove(orderId, out _);
        }
        else if (booking.Item2.HasFlag(Accepted.Rejected))
        {
            Console.WriteLine($"Guest {booking.Item1}, unfortunatelly no free table");
        }
    }
}

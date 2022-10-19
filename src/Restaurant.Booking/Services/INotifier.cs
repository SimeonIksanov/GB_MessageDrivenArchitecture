using System;
namespace Restaurant.Booking.Services;

public interface INotifier
{
    void NotifyAsync(string message);
}


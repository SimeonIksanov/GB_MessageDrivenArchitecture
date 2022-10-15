using System;
namespace RestaurantLib.Services;

public interface INotifier
{
    void NotifyAsync(string message);
}


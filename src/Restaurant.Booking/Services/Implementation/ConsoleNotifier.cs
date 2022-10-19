using System;
namespace Restaurant.Booking.Services.Implementation;

public class ConsoleNotifier : INotifier
{
    private readonly TimeSpan _delay;
    private readonly string _prefix;

    public ConsoleNotifier(TimeSpan delay)
    {
        _delay = delay;
        _prefix = "Notification: ";
    }

    public void NotifyAsync(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        Task.Run(async () =>
        {
            await Task.Delay(_delay);
            Console.WriteLine($"{_prefix}{message}");
        });
    }
}


namespace Restaurant.Notification;

public class Notifier
{
    public void Notify(Guid orderId, Guid clientId, string message)
    {
        Console.WriteLine($"[OrderId {orderId}]: Уважаемый клиент! {message}");
    }
}

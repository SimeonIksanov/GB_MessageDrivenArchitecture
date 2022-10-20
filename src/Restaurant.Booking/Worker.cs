using System.Diagnostics;
using System.Text;
using MassTransit;
using Messaging;
using Microsoft.Extensions.Hosting;
using Restaurant.Messaging;

namespace Restaurant.Booking;

internal class Worker : BackgroundService
{
    private readonly IBus _bus;
    private readonly Restaurant _restaurant;
    private CircuitBreaker _circuitBreaker;

    public Worker(IBus bus, Restaurant restaurant)
    {
        _bus = bus;
        _restaurant = restaurant;
        _circuitBreaker = new CircuitBreaker();
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(10_000, stoppingToken);
            Console.WriteLine("wanna book table?");
            var result = await _restaurant.BookFreeTableAsync(1);
            Dish dish = RandomDish();
            await _bus.Publish(new TableBooked(Guid.NewGuid(), Guid.NewGuid(), result, dish), stoppingToken);

            try
            {
                _circuitBreaker.Invoke(AskWhenDinner);
            }
            catch
            {
                Console.WriteLine("got exception from _circuitBreaker");
            }
        }
    }

    private void AskWhenDinner()
    {
        var requestClient = _bus.CreateRequestClient<KogdaObedRequest>();

        var task = requestClient.GetResponse<KogdaObedResponse>(
            new("kogda obed?"),
            CancellationToken.None);
        Console.WriteLine("Жду ответ");
        Task.WaitAll(task);
        Console.WriteLine("ответ пришел");
        Console.WriteLine(task.Result.Message.Body);
    }

    private Dish RandomDish()
    {
        var dishes = Enum.GetValues<Dish>(); ;
        int randomIndex = Random.Shared.Next(0, dishes.Length);
        return dishes[randomIndex];
    }
}

internal class CircuitBreaker
{
    private CircuitBreakerState _state = CircuitBreakerState.Closed;
    Queue<bool> _queue = new Queue<bool>(30);

    public void Invoke(Action action)
    {
        var stopWatch = new Stopwatch();

        if (_state == CircuitBreakerState.Open)
        {
            Console.WriteLine("CircuitBreaker Open");
            throw new Exception("CircuitBreaker Open");
        }

        stopWatch.Start();
        action.Invoke();
        stopWatch.Stop();

        _queue.Enqueue(stopWatch.ElapsedMilliseconds > 1500);
        if (_queue.Count > 30)
        {
            _ = _queue.Dequeue();
            if (_queue.Count(el => el == true) > 10)
            {
                _state = CircuitBreakerState.Open;
                Console.WriteLine("new CircuitBreaker state Open");
            }
            else
            {
                _state = CircuitBreakerState.Closed;
                Console.WriteLine("new CircuitBreaker state Closed");
            }
        }
    }

    private enum CircuitBreakerState
    {
        Open,
        HalfOpen,
        Closed
    }
}
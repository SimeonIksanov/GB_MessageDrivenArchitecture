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
    private Queue<bool> _queue = new Queue<bool>();
    private int _queueLen = 30;
    private int _maxFails = 10;
    private int _maxTimeout = 1500;
    private int _openPeriodInSeconds = 10;
    private DateTime _openedAt;
    private Stopwatch _stopWatch = new Stopwatch();

    public void Invoke(Action action)
    {
        if (_state == CircuitBreakerState.Open && (DateTime.Now - _openedAt).TotalSeconds > _openPeriodInSeconds)
        {
            Console.WriteLine("new CircuitBreaker state HalfOpen");
            _state = CircuitBreakerState.HalfOpen;
        }
        if (_state == CircuitBreakerState.Open)
        {
            Console.WriteLine("CircuitBreaker Open");
            throw new Exception("CircuitBreaker Open");
        }

        _stopWatch.Start();
        action.Invoke();
        _stopWatch.Stop();

        _queue.Enqueue(_stopWatch.ElapsedMilliseconds > _maxTimeout);
        if (_queue.Count > _queueLen)
        {
            _ = _queue.Dequeue();
            if (_queue.Count(el => el == true) > _maxFails)
            {
                _state = CircuitBreakerState.Open;
                _openedAt = DateTime.Now;
                Console.WriteLine("new CircuitBreaker state Open");
            }
            else
            {
                _state = CircuitBreakerState.Closed;
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
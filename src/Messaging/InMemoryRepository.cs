using System.Collections.Concurrent;

namespace Restaurant.Messages;

public class InMemoryRepository<T> : IModelRepository<T> where T : RequestModel
{
    private readonly ConcurrentBag<T> _bag = new ConcurrentBag<T>();
    private readonly Timer _timer;
    private int _cleanUpTimeout = 30;

    public InMemoryRepository()
    {
        _timer = new Timer(RepositoryCleaner, null, 0, 1000);
    }

    public void AddOrUpdate(T entity)
    {
        _bag.Add(entity);
    }

    public IEnumerable<T> Get()
    {
        return _bag;
    }

    private void RepositoryCleaner(object o)
    {
        //dirty hack =))
        int oldCount = _bag.Count;
        ConcurrentBag<T> tempBag = new ConcurrentBag<T>();
        while (!_bag.IsEmpty)
        {
            if (_bag.TryTake(out T item))
                if (DateTime.Now - item.CreatedAt < TimeSpan.FromSeconds(_cleanUpTimeout))
                    tempBag.Add(item);

        }
        foreach (var item in tempBag)
        {
            _bag.Add(item);
        }

        Console.WriteLine($"repo cleanup finished. Count before: {oldCount}, count after: {_bag.Count}");
    }
}

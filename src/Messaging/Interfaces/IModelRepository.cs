using System;

namespace Restaurant.Messages;

public interface IModelRepository<T>
{
    void AddOrUpdate(T entity);
    IEnumerable<T> Get();
}

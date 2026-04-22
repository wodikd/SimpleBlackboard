using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace SimpleBlackboard;

public class ConcurrentBlackboard<TKey> where TKey : notnull
{
    private readonly ConcurrentDictionary<Type, IStorage> _buckets = new();

    public void Set<TValue>(TKey key, TValue value)
    {
        GetOrCreateStorage<TValue>().Set(key, value);
    }

    public bool TryGet<TValue>(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (_buckets.TryGetValue(typeof(TValue), out var storage))
        {
            return ((ConcurrentStorage<TKey, TValue>)storage).TryGet(key, out value);
        }

        value = default;
        return false;
    }

    public bool ContainsKey<TValue>(TKey key)
    {
        return _buckets.TryGetValue(typeof(TValue), out var storage) && ((ConcurrentStorage<TKey, TValue>)storage).ContainsKey(key);
    }

    public bool TryRemove<TValue>(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (_buckets.TryGetValue(typeof(TValue), out var storage))
        {
            return ((ConcurrentStorage<TKey, TValue>)storage).TryRemove(key, out value);
        }
        
        value = default;
        return false;
    }

    public void Clear<TValue>()
    {
        if (_buckets.TryGetValue(typeof(TValue), out var storage))
        {
            storage.Clear();
        }
    }

    public void ClearAll()
    {
        foreach (var storage in _buckets.Values)
        {
            storage.Clear();
        }
    }

    private ConcurrentStorage<TKey, TValue> GetOrCreateStorage<TValue>() 
    {
        return (ConcurrentStorage<TKey, TValue>)_buckets.GetOrAdd(typeof(TValue), _ => new ConcurrentStorage<TKey, TValue>());
    }
}
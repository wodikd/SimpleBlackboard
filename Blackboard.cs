using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis; // 추가

namespace SimpleBlackboard;

public class Blackboard<TKey> : IBlackboard<TKey> where TKey : notnull
{
    private readonly Dictionary<Type, IStorage> _buckets = new();

    public void Set<TValue>(TKey key, TValue value)
    {
        GetOrCreateStorage<TValue>().Set(key, value);
    }

    public bool TryGet<TValue>(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (_buckets.TryGetValue(typeof(TValue), out var storage))
        {
            return ((Storage<TKey, TValue>)storage).TryGet(key, out value);
        }

        value = default;
        return false;
    }

    public bool ContainsKey<TValue>(TKey key)
    {
        return _buckets.TryGetValue(typeof(TValue), out var storage) && ((Storage<TKey, TValue>)storage).ContainsKey(key);
    }

    public bool Remove<TValue>(TKey key)
    {
        return _buckets.TryGetValue(typeof(TValue), out var storage) && ((Storage<TKey, TValue>)storage).Remove(key);
    }

    public bool Remove<TValue>(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        value = default;
        return _buckets.TryGetValue(typeof(TValue), out var storage) &&
               ((Storage<TKey, TValue>)storage).Remove(key, out value);
    }

    public bool TryRemove<TValue>(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (_buckets.TryGetValue(typeof(TValue), out var storage))
        {
            return ((Storage<TKey, TValue>)storage).TryRemove(key, out value);
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
        // _buckets.Clear()를 하지 않고 내부 딕셔너리만 비워 메모리(Capacity) 재사용
        foreach (var storage in _buckets.Values)
        {
            storage.Clear();
        }
    }
    
    public IEnumerable<Type> GetRegisteredTypes() => _buckets.Keys;

    public bool TryGetStorage<TValue>([MaybeNullWhen(false)] out IReadOnlyDictionary<TKey, TValue> dictionary)
    {
        if (_buckets.TryGetValue(typeof(TValue), out var storage))
        {
            dictionary = (IReadOnlyDictionary<TKey, TValue>)((Storage<TKey, TValue>)storage).Data;
            return true;
        }

        dictionary = null;
        return false;
    }

    private Storage<TKey, TValue> GetOrCreateStorage<TValue>() 
    {
        if (_buckets.TryGetValue(typeof(TValue), out var storage))
        {
            return (Storage<TKey, TValue>)storage;
        }

        var newStorage = new Storage<TKey, TValue>();
        _buckets.Add(typeof(TValue), newStorage);
        return newStorage;
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis; // 추가

namespace SimpleBlackboard;

public class Blackboard<TKey> where TKey : notnull
{
    private readonly Dictionary<Type, IStorage> _buckets = new();

    public void Set<T>(TKey key, T value)
    {
        GetOrCreateStorage<T>().Set(key, value);
    }

    public bool TryGet<T>(TKey key, [MaybeNullWhen(false)] out T value)
    {
        if (_buckets.TryGetValue(typeof(T), out var storage))
        {
            return ((Storage<T>)storage).TryGet(key, out value);
        }

        value = default;
        return false;
    }

    public bool ContainsKey<T>(TKey key)
    {
        return _buckets.TryGetValue(typeof(T), out var storage) && ((Storage<T>)storage).ContainsKey(key);
    }

    public bool Remove<T>(TKey key)
    {
        return _buckets.TryGetValue(typeof(T), out var storage) && ((Storage<T>)storage).Remove(key);
    }

    public bool TryRemove<T>(TKey key, [MaybeNullWhen(false)] out T value)
    {
        if (_buckets.TryGetValue(typeof(T), out var storage))
        {
            return ((Storage<T>)storage).TryRemove(key, out value);
        }
        
        value = default;
        return false;
    }

    public void Clear<T>()
    {
        if (_buckets.TryGetValue(typeof(T), out var storage))
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

    private Storage<T> GetOrCreateStorage<T>() 
    {
        if (_buckets.TryGetValue(typeof(T), out var storage))
        {
            return (Storage<T>)storage;
        }

        var newStorage = new Storage<T>();
        _buckets.Add(typeof(T), newStorage);
        return newStorage;
    }
    
    // Clear를 인터페이스로 끌어올림
    private interface IStorage 
    { 
        void Clear();
    }
    
    private class Storage<TValue> : IStorage
    {
        private readonly Dictionary<TKey, TValue> _data = new();
        
        public void Set(TKey key, TValue value) => _data[key] = value;
        public bool TryGet(TKey key, [MaybeNullWhen(false)] out TValue value) => _data.TryGetValue(key, out value);
        public bool ContainsKey(TKey key) => _data.ContainsKey(key);
        public bool Remove(TKey key) => _data.Remove(key);
        public bool TryRemove(TKey key, [MaybeNullWhen(false)] out TValue value) => _data.Remove(key, out value);
        public void Clear() => _data.Clear();
    }
}
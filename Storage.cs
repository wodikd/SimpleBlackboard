using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SimpleBlackboard;

internal interface IStorage<TKey> where TKey : notnull
{
    void Clear();
    void CopyTo(IBlackboard<TKey> destination);
}

internal interface IStorage<TKey, TValue> : IStorage<TKey> where TKey : notnull
{
    IReadOnlyDictionary<TKey, TValue> Data { get; }
    void Set(TKey key, TValue value);
    bool TryGet(TKey key, [MaybeNullWhen(false)] out TValue value);
    bool ContainsKey(TKey key);
    bool Remove(TKey key);
    bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value);
    bool TryRemove(TKey key, [MaybeNullWhen(false)] out TValue value);
}

internal class Storage<TKey, TValue> : IStorage<TKey, TValue> where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _data = new();
    public IReadOnlyDictionary<TKey, TValue> Data => _data;
        
    public void Set(TKey key, TValue value) => _data[key] = value;
    public bool TryGet(TKey key, [MaybeNullWhen(false)] out TValue value) => _data.TryGetValue(key, out value);
    public bool ContainsKey(TKey key) => _data.ContainsKey(key);
    public bool Remove(TKey key) => _data.Remove(key);
    public bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value) => _data.Remove(key, out value);
    public bool TryRemove(TKey key, [MaybeNullWhen(false)] out TValue value) => _data.Remove(key, out value);
    public void Clear() => _data.Clear();
    public void CopyTo(IBlackboard<TKey> destination)
    {
        foreach (var kvp in _data)
        {
            destination.Set(kvp.Key, kvp.Value);
        }
    }
}

internal class ConcurrentStorage<TKey, TValue> : IStorage<TKey, TValue> where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, TValue> _data = new();
    public IReadOnlyDictionary<TKey, TValue> Data => _data;
        
    public void Set(TKey key, TValue value) => _data[key] = value;
    public bool TryGet(TKey key, [MaybeNullWhen(false)] out TValue value) => _data.TryGetValue(key, out value);
    public bool ContainsKey(TKey key) => _data.ContainsKey(key);
    public bool Remove(TKey key) => _data.TryRemove(key, out _);
    public bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value) => _data.TryRemove(key, out value);
    public bool TryRemove(TKey key, [MaybeNullWhen(false)] out TValue value) => _data.TryRemove(key, out value);
    public void Clear() => _data.Clear();
    public void CopyTo(IBlackboard<TKey> destination)
    {
        foreach (var kvp in _data)
        {
            destination.Set(kvp.Key, kvp.Value);
        }
    }
}
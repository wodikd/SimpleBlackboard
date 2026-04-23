using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace SimpleBlackboard;

// Clear를 인터페이스로 끌어올림
internal interface IStorage 
{
    void Clear();
}
    
internal class Storage<TKey, TValue> : IStorage where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _data = new();
    public IDictionary<TKey, TValue> Data => _data;
        
    public void Set(TKey key, TValue value) => _data[key] = value;
    public bool TryGet(TKey key, [MaybeNullWhen(false)] out TValue value) => _data.TryGetValue(key, out value);
    public bool ContainsKey(TKey key) => _data.ContainsKey(key);
    public bool Remove(TKey key) => _data.Remove(key);
    public bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value) => _data.Remove(key, out value);
    public bool TryRemove(TKey key, [MaybeNullWhen(false)] out TValue value) => _data.Remove(key, out value);
    public void Clear() => _data.Clear();
}

internal class ConcurrentStorage<TKey, TValue> : IStorage where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, TValue> _data = new();
    public IDictionary<TKey, TValue> Data => _data;
        
    public void Set(TKey key, TValue value) => _data[key] = value;
    public bool TryGet(TKey key, [MaybeNullWhen(false)] out TValue value) => _data.TryGetValue(key, out value);
    public bool ContainsKey(TKey key) => _data.ContainsKey(key);
    public bool Remove(TKey key, [MaybeNullWhen(false)] out TValue value) => _data.Remove(key, out value);
    public bool TryRemove(TKey key, [MaybeNullWhen(false)] out TValue value) => _data.Remove(key, out value);
    public void Clear() => _data.Clear();
}
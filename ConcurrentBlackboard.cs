using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SimpleBlackboard;

public class ConcurrentBlackboard<TKey> : BlackboardBase<TKey> where TKey : notnull
{
    private readonly ConcurrentDictionary<Type, IStorage<TKey>> _buckets = new();

    internal override bool TryGetStorageObject(Type type, [MaybeNullWhen(false)] out IStorage<TKey> storage) 
        => _buckets.TryGetValue(type, out storage);

    internal override IStorage<TKey, TValue> GetOrCreateStorage<TValue>()
    {
        return (IStorage<TKey, TValue>)_buckets.GetOrAdd(typeof(TValue), _ => new ConcurrentStorage<TKey, TValue>());
    }

    internal override IEnumerable<IStorage<TKey>> GetAllStorages() => _buckets.Values;

    public override IEnumerable<Type> GetRegisteredTypes() => _buckets.Keys;
}
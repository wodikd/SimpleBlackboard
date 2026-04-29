using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SimpleBlackboard;

public class Blackboard<TKey> : BlackboardBase<TKey> where TKey : notnull
{
    private readonly Dictionary<Type, IStorage<TKey>> _buckets = new();

    internal override bool TryGetStorageObject(Type type, [MaybeNullWhen(false)] out IStorage<TKey> storage) 
        => _buckets.TryGetValue(type, out storage);

    internal override IStorage<TKey, TValue> GetOrCreateStorage<TValue>()
    {
        if (_buckets.TryGetValue(typeof(TValue), out var storage))
        {
            return (IStorage<TKey, TValue>)storage;
        }

        var newStorage = new Storage<TKey, TValue>();
        _buckets.Add(typeof(TValue), newStorage);
        return newStorage;
    }

    internal override IEnumerable<IStorage<TKey>> GetAllStorages() => _buckets.Values;

    public override IEnumerable<Type> GetRegisteredTypes() => _buckets.Keys;
}
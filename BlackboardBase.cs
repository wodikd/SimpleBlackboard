using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SimpleBlackboard;

public abstract class BlackboardBase<TKey> : IBlackboard<TKey> where TKey : notnull
{
    internal abstract bool TryGetStorageObject(Type type, [MaybeNullWhen(false)] out IStorage<TKey> storage);
    internal abstract IStorage<TKey, TValue> GetOrCreateStorage<TValue>();
    internal abstract IEnumerable<IStorage<TKey>> GetAllStorages();
    public abstract IEnumerable<Type> GetRegisteredTypes();

    private bool TryGetInternalStorage<TValue>([MaybeNullWhen(false)] out IStorage<TKey, TValue> storage)
    {
        if (TryGetStorageObject(typeof(TValue), out var obj))
        {
            storage = (IStorage<TKey, TValue>)obj;
            return true;
        }
        storage = null;
        return false;
    }

    public void Set<TValue>(TKey key, TValue value)
    {
        GetOrCreateStorage<TValue>().Set(key, value);
    }

    public bool TryGet<TValue>(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (TryGetInternalStorage<TValue>(out var storage))
        {
            return storage.TryGet(key, out value);
        }

        value = default;
        return false;
    }

    public bool ContainsKey<TValue>(TKey key)
    {
        return TryGetInternalStorage<TValue>(out var storage) && storage.ContainsKey(key);
    }

    public bool Remove<TValue>(TKey key)
    {
        return TryGetInternalStorage<TValue>(out var storage) && storage.Remove(key);
    }

    public bool Remove<TValue>(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (TryGetInternalStorage<TValue>(out var storage))
        {
            return storage.Remove(key, out value);
        }

        value = default;
        return false;
    }

    public bool TryRemove<TValue>(TKey key, [MaybeNullWhen(false)] out TValue value)
    {
        if (TryGetInternalStorage<TValue>(out var storage))
        {
            return storage.TryRemove(key, out value);
        }

        value = default;
        return false;
    }

    public void Clear<TValue>()
    {
        if (TryGetStorageObject(typeof(TValue), out var storage))
        {
            storage.Clear();
        }
    }

    public void ClearAll()
    {
        foreach (var storage in GetAllStorages())
        {
            storage.Clear();
        }
    }

    public void CopyTo(IBlackboard<TKey> destination)
    {
        if (destination == null) throw new ArgumentNullException(nameof(destination));
        foreach (var storage in GetAllStorages())
        {
            storage.CopyTo(destination);
        }
    }

    public bool TryGetStorage<TValue>([MaybeNullWhen(false)] out IReadOnlyDictionary<TKey, TValue> dictionary)
    {
        if (TryGetInternalStorage<TValue>(out var storage))
        {
            dictionary = storage.Data;
            return true;
        }

        dictionary = null;
        return false;
    }
}
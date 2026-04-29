using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SimpleBlackboard;

public interface IBlackboard<TKey> where TKey : notnull
{
    void Set<TValue>(TKey key, TValue value);
    bool TryGet<TValue>(TKey key, [MaybeNullWhen(false)] out TValue value);
    bool ContainsKey<TValue>(TKey key);
    bool Remove<TValue>(TKey key);
    bool Remove<TValue>(TKey key, [MaybeNullWhen(false)] out TValue value);
    bool TryRemove<TValue>(TKey key, [MaybeNullWhen(false)] out TValue value);
    void Clear<TValue>();
    void ClearAll();
    void CopyTo(IBlackboard<TKey> destination);
    IEnumerable<Type> GetRegisteredTypes();
    bool TryGetStorage<TValue>([MaybeNullWhen(false)] out IReadOnlyDictionary<TKey, TValue> dictionary);
}

using System.Diagnostics.CodeAnalysis;

namespace SimpleBlackboard;

public readonly struct BoardData<TKey, TValue> where TKey : notnull
{
    private readonly IBlackboard<TKey> _blackboard;
    private readonly TKey _key;
    
    public BoardData(IBlackboard<TKey> blackboard, TKey key)
    {
        _blackboard = blackboard ?? throw new ArgumentNullException(nameof(blackboard));
        _key = key;
    }

    public bool Exists => _blackboard.ContainsKey<TValue>(_key);

    // public TValue Value
    // {
    //     get
    //     {
    //         if (!_blackboard.TryGet(_key, out TValue? value))
    //         {
    //             throw new KeyNotFoundException($"Key '{_key}' of type '{typeof(TValue)}' not found in blackboard.");
    //         }
    //         return value;
    //     }
    //     set => _blackboard.Set(_key, value);
    // }

    public bool TryGet([MaybeNullWhen(false)] out TValue value) => _blackboard.TryGet(_key, out value);

    public bool Remove() => _blackboard.Remove<TValue>(_key);

    public override string ToString() => $"BoardData<{typeof(TValue).Name}>[{_key}]";
}
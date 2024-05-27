using System;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace WoWCombatLogParser;

internal class CombatLogVersionedDictionary<TKey, TValue>
    where TKey : notnull
    where TValue : notnull
{
    private readonly Dictionary<TKey, TValue> _allVersions = [];
    private readonly Dictionary<(CombatLogVersion, TKey), TValue> _specificVersions = [];

    [DisallowNull]
    public TValue? this[CombatLogVersion v, TKey k]
    {
        get => _allVersions.TryGetValue(k, out var value) ? value : _specificVersions.TryGetValue((v, k), out value) ? value : default;
        set => TryAdd(v, k, value);
    }

    public bool TryGetValue(CombatLogVersion combatLogVersion, TKey key, out TValue? value)
    {
        if (_allVersions.ContainsKey(key) || _specificVersions.ContainsKey((combatLogVersion, key)))
        {
            value = this[combatLogVersion, key];
            return true;
        }

        value = default;
        return false;
    }

    public bool TryAdd(CombatLogVersion combatLogVersion, TKey key, TValue value) => combatLogVersion switch
    {
        CombatLogVersion.Any => _allVersions.TryAdd(key, value),
        _ => _specificVersions.TryAdd((combatLogVersion, key), value),
    };

    public IEnumerable<T> GetKeys<T>(Func<KeyValuePair<(CombatLogVersion CombatLogVersion, TKey Discriminator), TValue>, T> selector)
    {
        return _allVersions.Select(x => KeyValuePair.Create((CombatLogVersion.Any, x.Key), x.Value))
            .Union(_specificVersions)
            .Select(selector);
    }
}

using System;
using System.Linq;
using System.Reflection;
using static WoWCombatLogParser.IO.CombatLogFieldReader;
using WoWCombatLogParser.IO;
using System.Diagnostics.CodeAnalysis;

namespace WoWCombatLogParser;

public partial interface IEventGenerator
{
    IParserContext ParserContext { get; set; }
    CombatLogVersionEvent CombatLogVersionEvent { get; }
    void SetCombatLogVersion(string combatLogVersion);
    List<string> GetRegisteredEventHandlers();
    CombatLogEvent? GetCombatLogEvent(string line);
    CombatLogEvent? GetCombatLogEvent(CombatLogLineData line);
    T? GetCombatLogEvent<T>(string line) where T : CombatLogEvent;
    T? GetCombatLogEvent<T>(CombatLogLineData line) where T : CombatLogEvent;
}


public class EventGenerator : IEventGenerator
{
    private static readonly CombatLogVersionedDictionary<string, ObjectActivator> _ctors = new();
    private static readonly Assembly _assembly = Assembly.Load("WoWCombatLogParser.Common");
    private static readonly CombatLogEventMapper mapper = new();

    static EventGenerator()
    {
        SetupCombatLogEvents();
    }

    public EventGenerator()
    {
    }

    public CombatLogVersionEvent CombatLogVersionEvent { get; set; }

    public IParserContext ParserContext { get; set; }

    public CombatLogEvent? GetCombatLogEvent(string line) => GetCombatLogEvent<CombatLogEvent>(line);
    public CombatLogEvent? GetCombatLogEvent(CombatLogLineData line) => GetCombatLogEvent<CombatLogEvent>(line);
    public T? GetCombatLogEvent<T>(string line) where T : CombatLogEvent => GetCombatLogEvent<T>(ReadFields(line));
    public T? GetCombatLogEvent<T>(CombatLogLineData line) where T : CombatLogEvent
    {
        var result = GetInstanceOf(line.EventType);
        if (result is { })
            mapper[result.GetType()]!(result, line.Data, 0);
        return (T?)result;
    }

    private CombatLogEvent GetInstanceOf(string eventType)
    {
        var constructor = _ctors[CombatLogVersionEvent.Version, eventType] ??
            throw new CombatLogParserException(eventType, new ArgumentOutOfRangeException(nameof(eventType), $"No constructor could be found for an event of type \"{eventType}\""));
        var result = (CombatLogEvent)constructor();
        if (result is { })
        {            
            return result;
        }
        throw new CombatLogParserException(eventType, new InvalidOperationException($"Failed to instantiate an instance for an event for \"{eventType}\""));
    }
    
    public List<string> GetRegisteredEventHandlers() => [.. _ctors.GetKeys(x => x.Key.Discriminator).OrderBy(x => x)];
    
    private static void SetupCombatLogEvents()
    {
        GetTypesWhere(i => i.GetCustomAttribute<AffixAttribute>() != null)
            .ToList()
            .ConvertAll(i => new EventAffixItem(i))
            .ForEach(p => AddType(p.Affix.Name, p.EventType));
    }

    private static void AddType(string name, Type type)
    {
        var constructor = type.GetConstructors().First();
        var activator = CombatLogEventActivator.GetActivator(constructor);

        var applicableCombatLogVersions = type
            .GetCustomAttributes<CombatLogVersionAttribute>()
            .Select(x => x.Value)
            .ToList();
        if (!applicableCombatLogVersions.Any())
            applicableCombatLogVersions = [CombatLogVersion.Any];

        foreach (var CombatLogVersion in applicableCombatLogVersions)
        {
            _ctors.TryAdd(CombatLogVersion, name, activator);
        }
    }

    private static IEnumerable<Type> GetTypesWhere(Func<Type, bool> expr)
    {
        foreach (var type in _assembly.GetTypes().Where(expr))
            yield return type;
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(expr))
            yield return type;
    }

    public void SetCombatLogVersion(string combatLogVersion)
    {
        CombatLogVersionEvent = new CombatLogVersionEvent(combatLogVersion);
    }
}

public class CombatLogVersionedDictionary<TKey, TValue>
    where TKey: notnull
    where TValue: notnull
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

public class CombatLogParserException(string eventType, Exception innerException)
    : Exception(innerException.Message, innerException)
{
    public string EventType { get; set; } = eventType;
}
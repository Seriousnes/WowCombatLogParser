﻿using System;
using System.Linq;
using System.Reflection;
using WoWCombatLogParser.Common.Models;
using WoWCombatLogParser.Common.Utility;
using WoWCombatLogParser.Common.Events;
using static WoWCombatLogParser.IO.CombatLogFieldReader;
using System.Threading.Tasks;
using WoWCombatLogParser.IO;

namespace WoWCombatLogParser;

public partial interface IEventGenerator
{
    IParserContext ParserContext { get; set; }
    CombatLogVersionEvent CombatLogVersionEvent { get; }
    ClassMap GetClassMap(Type type);
    void SetCombatLogVersion(string combatLogVersion);
    List<string> GetRegisteredEventHandlers();
    List<string> GetRegisteredClassMaps();
    T GetCombatLogEvent<T>(CombatLogLineData line, Action<ICombatLogEvent>? afterCreate = null) where T : CombatLogEvent;
    T GetCombatLogEvent<T>(string line, Action<ICombatLogEvent>? afterCreate = null) where T : CombatLogEvent;
    Task<T> GetCombatLogEventAsync<T>(CombatLogLineData line, Action<ICombatLogEvent>? afterCreate = null) where T : CombatLogEvent;
    Task<T> GetCombatLogEventAsync<T>(string line, Action<ICombatLogEvent>? afterCreate = null) where T : CombatLogEvent;
    CombatLogEvent GetCombatLogEvent(CombatLogLineData line, Action<ICombatLogEvent>? afterCreate = null);
    CombatLogEvent? GetCombatLogEvent(string line, Action<ICombatLogEvent>? afterCreate = null);
}

public class EventGenerator : IEventGenerator
{
    private static readonly CombatLogVersionedDictionary<string, ObjectActivator> _ctors = new();
    private static readonly CombatLogVersionedDictionary<Type, ClassMap> _classMap = new();
    private static readonly Assembly _assembly = Assembly.Load("WoWCombatLogParser.Common");


    static EventGenerator()
    {
        SetupClassMap();
        SetupCombatLogEvents();
    }

    public EventGenerator()
    {
    }

    public CombatLogVersionEvent CombatLogVersionEvent { get; set; }

    public IParserContext ParserContext { get; set; }

    public CombatLogEvent? GetCombatLogEvent(string line, Action<ICombatLogEvent>? afterCreate = null)
    {
        return GetCombatLogEvent<CombatLogEvent>(ReadFields(line), afterCreate);
    }

    public CombatLogEvent GetCombatLogEvent(CombatLogLineData line, Action<ICombatLogEvent>? afterCreate = null)
    {
        return GetCombatLogEventAsync<CombatLogEvent>(line, afterCreate).Result;
    }

    public T? GetCombatLogEvent<T>(string line, Action<ICombatLogEvent>? afterCreate = null) where T : CombatLogEvent
    {
        return GetCombatLogEvent<T>(ReadFields(line), afterCreate);
    }

    public T? GetCombatLogEvent<T>(CombatLogLineData line, Action<ICombatLogEvent>? afterCreate = null) where T : CombatLogEvent
    {
        return GetCombatLogEventAsync<T>(line, afterCreate).Result;
    }

    public async Task<T?> GetCombatLogEventAsync<T>(string line, Action<ICombatLogEvent>? afterCreate = null) where T : CombatLogEvent
    {
        return await GetCombatLogEventAsync<T>(ReadFields(line), afterCreate);
    }

    public async Task<T?> GetCombatLogEventAsync<T>(CombatLogLineData line, Action<ICombatLogEvent>? afterCreate = null) where T : CombatLogEvent
    {
        var result = GetInstanceOf<T>(line.EventType);
        if (result is { })
            await result.Parse(line.Data);
        return result;
    }

    private T? GetInstanceOf<T>(string eventType) where T : CombatLogEvent
    {
        try
        {
            var constructor = _ctors[CombatLogVersionEvent.Version, eventType];
            if (constructor == null) return null;
            var result = (T)constructor();
            if (result is { })
            {
                result.ParserContext = ParserContext;
            }
            return result;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public ClassMap GetClassMap(Type type) => _classMap.TryGetValue(CombatLogVersionEvent.Version, type, out var value) ? value : null;

    public List<string> GetRegisteredEventHandlers() => _ctors.GetKeys(x => x.Key.Discriminator).OrderBy(x => x).ToList();

    public List<string> GetRegisteredClassMaps() => _classMap.GetKeys(x => x.Key.Discriminator.FullName).OrderBy(x => x).ToList();

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
        var activator = CombatLogEventActivator.GetActivator<object>(constructor);

        var applicableCombatLogVersions = type
            .GetCustomAttributes<CombatLogVersionAttribute>()
            .Select(x => x.Value)
            .ToList();
        if (!applicableCombatLogVersions.Any())
            applicableCombatLogVersions = new List<CombatLogVersion>() { CombatLogVersion.Any };

        foreach (var CombatLogVersion in applicableCombatLogVersions)
        {
            _ctors.TryAdd(CombatLogVersion, name, activator);
            _classMap.TryAdd(CombatLogVersion, type, new ClassMap { Constructor = activator, Properties = type.GetTypePropertyInfo().ToArray(), CustomAttributes = type.GetCustomAttributes().ToList() });
        }
    }

    private static void SetupClassMap()
    {
        var types = GetTypesWhere(i => i.GetCustomAttribute<AffixAttribute>() == null && i.IsSubclassOf(typeof(CombatLogEventComponent)) && !i.IsAbstract && !i.IsGenericType);
        var classMaps = types
            .ToDictionary(key => key, value => new ClassMap
            {
                Constructor = CombatLogEventActivator.GetActivator<CombatLogEventComponent>(value.GetConstructors().FirstOrDefault()),
                Properties = value.GetTypePropertyInfo().ToList(),
                CustomAttributes = value.GetCustomAttributes().ToList()
            })
            .ToList();

        foreach ((Type t, ClassMap cMap) in classMaps)
        {
            var applicableCombatLogVersions = t.GetCustomAttributes<CombatLogVersionAttribute>()
                .Select(x => x.Value)
                .ToList();
            if (!applicableCombatLogVersions.Any())
                applicableCombatLogVersions = new List<CombatLogVersion>() { CombatLogVersion.Any };

            foreach (var CombatLogVersion in applicableCombatLogVersions)
                _classMap.TryAdd(CombatLogVersion, t, cMap);
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
{
    private readonly Dictionary<TKey, TValue> _allVersions = new();
    private readonly Dictionary<(CombatLogVersion, TKey), TValue> _specificVersions = new();

    public TValue this[CombatLogVersion v, TKey k]
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
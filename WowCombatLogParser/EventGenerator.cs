using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using WoWCombatLogParser.Common.Models;
using WoWCombatLogParser.Common.Utility;
using WoWCombatLogParser.Common.Events;
using System.Collections;
using System.Buffers;
using System.Text;

namespace WoWCombatLogParser
{
    public partial class EventGenerator : IEventGenerator
    {
        [GeneratedRegex("(?<timestamp>.*?)\\s{2}(?<eventtype>.*?),(?<parameters>.*)", RegexOptions.Compiled)]
        private static partial Regex getEventTypeExpr();
        private static readonly Regex _eventTypeExpr = getEventTypeExpr(); 
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

        public IApplicationContext ApplicationContext { get; set; }

        public T GetCombatLogEvent<T>(string line, Action<ICombatLogEvent> afterCreate = null) where T : class, ICombatLogEvent
        {
            var preParseResult = PreParseLine(line);
            var ctor = _ctors[CombatLogVersionEvent.Version, preParseResult.Event];
            if (ctor == null) return null;

            var result = (T)ctor(preParseResult.Parameters, ApplicationContext);
            afterCreate?.Invoke(result);
            return result;
        }
        
        public CombatLogVersionEvent GetCombatLogVersionEvent(string line, Action<ICombatLogEvent> afterCreate = null)
        {
            var result = new CombatLogVersionEvent(line, ApplicationContext);            
            return result;
        }

        private (string Event, string Parameters) PreParseLine(string line)
        {
            var m = _eventTypeExpr.Match(line).Groups;
            return (m["eventtype"].Value, string.Join(',', m["timestamp"].Value, m["parameters"].Value));
        }

        public T CreateEventSection<T>()
        {
            var ctor = _classMap[CombatLogVersionEvent.Version, typeof(T)]?.Constructor;
            if (ctor == null) return default;
            return (T)ctor();
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
            var constructor = type.GetConstructor(new[] { typeof(string), typeof(IApplicationContext) }) ?? type.GetConstructors().First();
            var activator = CombatLogEventActivator.GetActivator<object>(constructor);

            var applicableCombatLogVersions = type.GetCustomAttributes<CombatLogVersionAttribute>()
                .Select(x => x.Value)
                .ToList();
            if (!applicableCombatLogVersions.Any())
                applicableCombatLogVersions = new List<CombatLogVersion>() { CombatLogVersion.Any };

            foreach (var CombatLogVersion in applicableCombatLogVersions)
            {
                _ctors.TryAdd(CombatLogVersion, name, activator);
                _classMap.TryAdd(CombatLogVersion, type, new ClassMap { Constructor = activator, Properties = type.GetTypePropertyInfo().ToArray() });
            }
        }

        private static void SetupClassMap()
        {
            var types = GetTypesWhere(i => i.GetCustomAttribute<AffixAttribute>() == null && i.IsSubclassOf(typeof(EventSection)) && !i.IsAbstract && !i.IsGenericType);
            var classMaps = types
                .ToDictionary(key => key, value => new ClassMap
                {
                    Constructor = CombatLogEventActivator.GetActivator<EventSection>(value.GetConstructors().FirstOrDefault()),
                    Properties = value.GetTypePropertyInfo().ToList()
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
            var (_, parameters) = PreParseLine(combatLogVersion);
            CombatLogVersionEvent = GetCombatLogVersionEvent(parameters);
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

        public bool TryGetValue(CombatLogVersion combatLogVersion, TKey key, out TValue value)
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
}

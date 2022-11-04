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

namespace WoWCombatLogParser
{
    public class EventGenerator : IEventGenerator
    {
        private static readonly VersionedDictionary<string, ObjectActivator> _ctors = new();
        private static readonly VersionedDictionary<Type, ClassMap> _classMap = new();
        private static readonly Assembly _assembly = Assembly.Load("WoWCombatLogParser.Common");
        private static readonly Regex _split = new(@"\s\s", RegexOptions.Compiled);

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
            var (timestamp, eventType) = GetMinimalEventDetails(ref line);
            var ctor = _ctors[CombatLogVersionEvent.Version].Where(c => c.Key == eventType).Select(c => c.Value).SingleOrDefault();
            if (ctor == null) return null;

            var result = (T)ctor(timestamp, eventType, line);
            afterCreate?.Invoke(result);
            return result;
        }

        public CombatLogVersionEvent GetCombatLogVersionEvent(string line, Action<ICombatLogEvent> afterCreate = null)
        {
            var (timestamp, eventType) = GetMinimalEventDetails(ref line);
            var result = new CombatLogVersionEvent(timestamp, eventType, line);
            return result;
        }

        public T CreateEventSection<T>()
        {
            var ctor = _classMap[CombatLogVersionEvent.Version].Where(t => t.Key == typeof(T)).Select(c => c.Value.Constructor).SingleOrDefault();
            if (ctor == null) return default;
            return (T)ctor();
        }

        private (DateTime Timestamp, string EventType) GetMinimalEventDetails(ref string line)
        {
            var i = line.IndexOf(',');
            var substr = line.Substring(0, i++);
            line = new string(line.Skip(i).ToArray());
            var resultParts = _split.Split(substr).ToArray();
            return (DateTime.ParseExact(resultParts[(int)FieldIndex.Timestamp], "M/d HH:mm:ss.fff", CultureInfo.InvariantCulture), resultParts[(int)FieldIndex.EventType]);
        }

        public ClassMap GetClassMap(Type type) => _classMap[CombatLogVersionEvent.Version].TryGetValue(type, out var value) ? value : null;
        
        public List<string> GetRegisteredEventHandlers() => _ctors[CombatLogVersionEvent.Version].Select(x => x.Key).OrderBy(x => x).ToList();

        public List<string> GetRegisteredClassMaps() => _classMap[CombatLogVersionEvent.Version].Select(x => x.Key.FullName).OrderBy(x => x).ToList();

        private static void SetupCombatLogEvents()
        {
            GetTypesWhere(i => i.GetCustomAttribute<AffixAttribute>() != null)
                .ToList()
                .ConvertAll(i => new EventAffixItem(i))
                .ForEach(p => AddType(p.Affix.Name, p.EventType));
        }

        private static void AddType(string name, Type type)
        {
            var constructor = type.GetConstructor(new[] { typeof(DateTime), typeof(string), typeof(string) }) ?? type.GetConstructors().First();
            var activator = CombatLogEventActivator.GetActivator<object>(constructor);

            var applicableCombatLogVersions = type.GetCustomAttributes<CombatLogVersionAttribute>()
                .Select(x => x.Value)
                .ToList();
            if (!applicableCombatLogVersions.Any())
                applicableCombatLogVersions = new List<CombatLogVersion>() { CombatLogVersion.Any };

            foreach (var CombatLogVersion in applicableCombatLogVersions)
            {
                _ctors.TryAdd(name, activator, CombatLogVersion);
                _classMap.TryAdd(type, new ClassMap { Constructor = activator, Properties = type.GetTypePropertyInfo().ToArray() }, CombatLogVersion);
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
                    _classMap.TryAdd(t, cMap, CombatLogVersion);
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
            CombatLogVersionEvent = GetCombatLogVersionEvent(combatLogVersion, e => ApplicationContext.CombatLogParser.Parse(e));
            ApplicationContext.CombatLogParser.Parse(CombatLogVersionEvent);
        }
    }

    public class VersionedDictionary<TKey, TValue> : Dictionary<TKey, Dictionary<CombatLogVersion, TValue>>
    {
        public Dictionary<TKey, TValue> this[CombatLogVersion index] => this.ToDictionary(x => x.Key, x => x.Value.Where(c => c.Key.In(CombatLogVersion.Any, index)).Select(x => x.Value).SingleOrDefault());

        public bool TryAdd(TKey key, TValue value, CombatLogVersion version)
        {
            if (!TryGetValue(key, out var keyList))
            {
                keyList = new Dictionary<CombatLogVersion, TValue>();
                Add(key, keyList);
            }

            return keyList.TryAdd(version, value);
        }
    }
}

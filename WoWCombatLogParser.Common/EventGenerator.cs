using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using WoWCombatLogParser.Common.Events;
using WoWCombatLogParser.Common.Models;
using WoWCombatLogParser.Common.Utility;

namespace WoWCombatLogParser
{
    public static class EventGenerator
    {
        private static readonly Dictionary<string, ObjectActivator> _ctors = new Dictionary<string, ObjectActivator>();
        private static readonly Dictionary<Type, ClassMap> _classMap = new Dictionary<Type, ClassMap>();
        private static readonly Assembly _assembly = Assembly.Load("WoWCombatLogParser");
        private static readonly Regex _split = new Regex(@"\s\s", RegexOptions.Compiled);
        
        static EventGenerator()
        {
            SetupClassMap();
            SetupCombatLogEvents();
        }

        public static T GetCombatLogEvent<T>(string line) where T : class
        {
            var details = GetMinimalEventDetails(ref line);
            var ctor = _ctors.Where(c => c.Key == details.EventType).Select(c => c.Value).SingleOrDefault();
            if (ctor == null) return null;
            return (T)ctor(details.Timestamp, details.EventType, line);
        }

        public static T CreateEventSection<T>()
        {
            var ctor = _classMap.Where(t => t.Key == typeof(T)).Select(c => c.Value.Constructor).SingleOrDefault();
            if (ctor == null) return default;
            return (T)ctor();
        }

        private static (DateTime Timestamp, string EventType) GetMinimalEventDetails(ref string line)
        {
            var i = line.IndexOf(',');
            var substr = line.Substring(0, i++);
            line = new string(line.Skip(i).ToArray());
            var resultParts = _split.Split(substr).ToArray();
            return (DateTime.ParseExact(resultParts[(int)FieldIndex.Timestamp], "M/d HH:mm:ss.fff", CultureInfo.InvariantCulture), resultParts[(int)FieldIndex.EventType]);

        }

        public static ClassMap GetClassMap(Type type) => _classMap.TryGetValue(type, out var value) ? value : null;

        public static List<string> GetRegisteredEventHandlers() => _ctors.Select(x => x.Key).OrderBy(x => x).ToList();

        private static void SetupCombatLogEvents()
        {
            GetTypesWhere(i => i.GetCustomAttribute<AffixAttribute>() != null)
                .ToList()
                .ConvertAll(i => new EventAffixItem(i))
                .ForEach(p => AddType(p.Affix.Name, p.EventType));            
        }

        private static void AddType(string name, Type type)
        {
            var constructor = type.GetConstructor(new[] { typeof(DateTime), typeof(string), typeof(string) });            
            if (constructor == null)
            {
                constructor = type.GetConstructors().First();
            }
            var activator = CombatLogEventActivator.GetActivator<object>(constructor);
            _ctors.Add(name, activator);
            _classMap.TryAdd(type, new ClassMap { Constructor = activator, Properties = type.GetTypePropertyInfo().ToArray() });
        }

        private static void SetupClassMap()
        {
            GetTypesWhere(i => i.GetCustomAttribute<AffixAttribute>() == null && i.IsSubclassOf(typeof(EventSection)) && !i.IsAbstract && !i.IsGenericType)
                .ToDictionary(key => key, value => new ClassMap
                {
                    Constructor = CombatLogEventActivator.GetActivator<EventSection>(value.GetConstructors().First()),
                    Properties = value.GetTypePropertyInfo().ToList()
                })
                .ToList()
                .ForEach(x => _classMap.Add(x.Key, x.Value));
        }

        private static IEnumerable<Type> GetTypesWhere(Func<Type, bool> expr)
        {
            foreach(var type in _assembly.GetTypes().Where(expr))
                yield return type;
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(expr))
                yield return type;
        }   
    }

    public class ClassMap
    {
        public ObjectActivator Constructor { get; set; }
        public IList<PropertyInfo> Properties { get; set; }
    }
}

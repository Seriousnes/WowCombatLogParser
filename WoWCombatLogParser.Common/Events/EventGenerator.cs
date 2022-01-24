using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WoWCombatLogParser.Common.Models;
using WoWCombatLogParser.IO;
using WoWCombatLogParser.Common.Utility;

namespace WoWCombatLogParser.Common.Events
{
    public static class EventGenerator
    {
        private static Dictionary<string, ObjectActivator> _ctors = new Dictionary<string, ObjectActivator>();
        private static Dictionary<Type, ClassMap> _classMap;
        private static Assembly _assembly = Assembly.Load("WoWCombatLogParser");

        static EventGenerator()
        {
            SetupClassMap();
            SetupCombatLogEvents();
        }

        public static T GetCombatLogEvent<T>(IList<IField> line) where T : class
        {
            var ctor = _ctors.Where(c => c.Key == line[(int)FieldId.EventType].AsString()).Select(c => c.Value).SingleOrDefault();
            if (ctor == null) return null;
            return (T)ctor(line);
        }

        public static T CreateEventSection<T>()
        {
            var ctor = _classMap.Where(t => t.Key == typeof(T)).Select(c => c.Value.Constructor).SingleOrDefault();
            if (ctor == null) return default;
            return (T)ctor();
        }

        public static ClassMap GetClassMap(Type type) => _classMap.TryGetValue(type, out var value) ? value : null;

        public static List<string> GetRegisteredEventHandlers() => _ctors.Select(x => x.Key).OrderBy(x => x).ToList();

        private static void SetupCombatLogEvents()
        {
            /*var events = */GetTypesWhere(i => i.GetCustomAttribute<AffixAttribute>() != null)
                .ToList()
                .ConvertAll(i => new EventAffixItem(i))
                .ForEach(p => AddType(p.Affix.Name, p.EventType));

            //events.ToList().ForEach(p => AddType(p.Affix.Name, p.EventType));
        }

        private static void AddType(string name, Type type)
        {
            var constructor = type.GetConstructor(new[] { typeof(IList<IField>) });            
            if (constructor == null)
            {
                constructor = type.GetConstructors().First();
            }
            var activator = CombatLogActivator.GetActivator<object>(constructor);
            _ctors.Add(name, activator);
            _classMap.TryAdd(type, new ClassMap { Constructor = activator, Properties = type.GetTypePropertyInfo().ToArray() });
        }

        private static void SetupClassMap()
        {
            _classMap = GetTypesWhere(i => i.GetCustomAttribute<AffixAttribute>() == null && i.IsSubclassOf(typeof(EventSection)) && !i.IsAbstract && !i.IsGenericType)
                .ToDictionary(key => key, value => new ClassMap
                {
                    Constructor = CombatLogActivator.GetActivator<EventSection>(value.GetConstructors().First()),
                    Properties = value.GetTypePropertyInfo().ToList()
                });
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

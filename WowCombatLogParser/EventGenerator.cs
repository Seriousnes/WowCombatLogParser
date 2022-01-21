using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WoWCombatLogParser.Events;
using WoWCombatLogParser.IO;
using WoWCombatLogParser.Utility;

namespace WoWCombatLogParser
{
    public static class EventGenerator
    {
        private static Dictionary<string, ObjectActivator> _ctors = new();
        private static Dictionary<Type, ClassMap> _classMap;

        static EventGenerator()
        {
            SetupClassMap();
            SetupCombatLogEvents();
        }

        public static CombatLogEvent GetCombatLogEvent(IList<IField> line)
        {
            var ctor = _ctors.Where(c => c.Key == line[(int)FieldId.EventType].AsString()).Select(c => c.Value).SingleOrDefault();
            if (ctor == null) return null;
            return (CombatLogEvent)ctor(line);
        }

        public static T CreateEventSection<T>()
        {
            var ctor = _classMap.Where(t => t.Key == typeof(T)).Select(c => c.Value.Constructor).SingleOrDefault();
            if (ctor == null) return default(T);
            return (T)ctor();
        }

        public static ClassMap GetClassMap(Type type) => _classMap.TryGetValue(type, out var value) ? value : null;

        public static List<string> GetRegisteredEventHandlers() => _ctors.Select(x => x.Key).OrderBy(x => x).ToList();

        private static void SetupCombatLogEvents()
        {
            var events = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(i => i.GetCustomAttribute<AffixAttribute>() != null)
                .ToList()
                .ConvertAll(i => new EventAffixItem(i));

            var suffixEvents = events.Where(i => i.IsSuffix);
            events.Where(i => !i.IsSuffix)
                .ToList()
                .ForEach(p =>
                {
                    if (p.IsSpecial)
                    {
                        var (name, constructorDefinition, constructorTypeParams) = p.GetSpecialConstructorTypeDefinition();
                        AddType(name, constructorDefinition, constructorTypeParams);
                    }
                    else
                    {
                        var suffixes = p.HasRestrictedSuffixes ? p.RestrictedSuffixes : suffixEvents;
                        suffixes.Where(s => !p.CheckSuffixIsAllowed(s.EventType))
                            .ToList()
                            .ForEach(s => AddType($"{p.Affix.Name}{s.Affix.Name}", typeof(CombatLogEvent<,>), p.EventType, s.EventType));
                    }
                });
        }

        private static void AddType(string name, Type type, params Type[] typeArguments)
        {
            var genericType = type.MakeGenericType(typeArguments);
            var activator = CombatLogActivator.GetActivator<CombatLogEvent>(genericType.GetConstructors().First());
            _ctors.Add(name, activator);
            
            // Try to add generic type definition, some predefined types may reuse the same definition
            _classMap.TryAdd(genericType, new ClassMap { Constructor = activator, Properties = genericType.GetTypePropertyInfo().ToArray() });
        }

        private static void SetupClassMap()
        {
            _classMap = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(i => i.IsSubclassOf(typeof(EventSection)) && !i.IsAbstract && !i.IsGenericType)
                .ToDictionary(key => key, value => new ClassMap
                {
                    Constructor = CombatLogActivator.GetActivator<EventSection>(value.GetConstructors().First()),
                    Properties = value.GetTypePropertyInfo().ToArray()
                });
        }
    }

    public class ClassMap
    {
        public ObjectActivator Constructor { get; set; }
        public PropertyInfo[] Properties { get; set; }
    }
}

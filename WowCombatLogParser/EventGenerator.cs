using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WoWCombatLogParser.Events;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser
{
    public static class EventGenerator
    {
        private static Dictionary<string, ObjectActivator<CombatLogEvent>> _ctors = new();
        private static Dictionary<Type, List<PropertyInfo>> _classMap;

        static EventGenerator()
        {
            ConfigureClassMap();
            ConfigureValidCombatLogEvents();
        }

        public static CombatLogEvent GetCombatLogEvent(IList<object> line)
        {
            var ctor = _ctors.Where(c => c.Key == (string)line[(int)FieldId.EventType]).Select(c => c.Value).SingleOrDefault();
            if (ctor == null) return null;
            return ctor(line);
        }      

        public static IList<PropertyInfo> GetClassMap(Type type)
        {
            if (_classMap.ContainsKey(type))
            {
                return _classMap[type];
            }

            return new List<PropertyInfo>();
        }

        private static void ConfigureValidCombatLogEvents()
        {
            var events = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(i => i.GetCustomAttribute<AffixAttribute>() != null)
                .ToList()
                .ConvertAll(i => new EventAffixItem(i));

            var prefixEvents = events.Where(i => i.IsPrefix);
            var suffixEvents = events.Where(i => i.IsSuffix);

            foreach (var prefix in prefixEvents)
            {
                var suffixes = prefix.HasRestrictedSuffixes ? prefix.RestrictedSuffixes : suffixEvents;
                foreach (var suffix in suffixes)
                {
                    if (prefix.CheckSuffixIsAllowed(suffix.EventType))
                        continue;

                    AddType($"{prefix.Affix.Name}{suffix.Affix.Name}", typeof(CombatLogEvent<,>), prefix.EventType, suffix.EventType);
                }
            }

            events.Where(i => i.IsSimple)
                .ToList()
                .ForEach(i => AddType(i.Affix.Name, typeof(CombatLogEvent<>), i.EventType));
        }

        private static void AddType(string name, Type type, params Type[] typeArguments)
        {
            var genericType = type.MakeGenericType(typeArguments);
            _ctors.Add(name, CombatLogActivator.GetActivator<CombatLogEvent>(genericType.GetConstructors().First()));
            _classMap.Add(genericType, GetTypePropertyInfo(genericType));
        }
    
        private static void ConfigureClassMap()
        {
            _classMap = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(i => typeof(IEventSection).IsAssignableFrom(i) && !i.IsAbstract && !i.IsGenericType)                
                .ToDictionary(key => key, value => GetTypePropertyInfo(value));            
        }

        private static List<PropertyInfo> GetTypePropertyInfo(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(i => i.GetCustomAttribute<NonDataAttribute>() == null)
                .OrderBy(i => i.DeclaringType == type)
                .ToList(); ;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WoWCombatLogParser.Events;
using WoWCombatLogParser.Models;
using WoWCombatLogParser.Utility;

namespace WoWCombatLogParser
{
    public static class EventGenerator
    {
        private static Dictionary<string, ObjectActivator<CombatLogEvent>> _ctors = new();
        private static Dictionary<Type, List<PropertyInfo>> _classMap;

        static EventGenerator()
        {
            SetupClassMap();
            SetupCombatLogEvents();
        }

        public static CombatLogEvent GetCombatLogEvent(IList<string> line)
        {
            var ctor = _ctors.Where(c => c.Key == line[(int)FieldId.EventType]).Select(c => c.Value).SingleOrDefault();
            if (ctor == null) return null;
            return ctor(line);
        }

        public static IList<PropertyInfo> GetClassMap(Type type) => _classMap.TryGetValue(type, out var value) ? value : new List<PropertyInfo>();

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
                        AddType(p.Affix.Name, typeof(CombatLogEvent<>), p.EventType);
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
            _ctors.Add(name, CombatLogActivator.GetActivator<CombatLogEvent>(genericType.GetConstructors().First()));
            _classMap.Add(genericType, genericType.GetTypePropertyInfo());
        }
    
        private static void SetupClassMap()
        {
            _classMap = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(i => i.IsSubclassOf(typeof(Part)) && !i.IsAbstract && !i.IsGenericType)                
                .ToDictionary(key => key, value => value.GetTypePropertyInfo());            
        }
    }    
}

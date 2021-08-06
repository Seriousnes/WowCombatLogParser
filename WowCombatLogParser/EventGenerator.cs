using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser
{
    public static class EventGenerator
    {
        static EventGenerator()
        {
            var events = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(i => i.GetCustomAttribute<AffixAttribute>() != null)
                .ToList()
                .ConvertAll(i => new EventAffixItem(i));

            var prefixEvents = events.Where(i => i.IsPrefix);
            var suffixEvents = events.Where(i => i.IsSuffix);

            var complexType = typeof(CombatLogEvent<,>);
            foreach (var prefix in prefixEvents)
            {
                var suffixes = prefix.HasRestrictedSuffixes ? prefix.RestrictedSuffixes : suffixEvents;
                foreach (var suffix in suffixes)
                {
                    if (prefix.CheckSuffixIsAllowed(suffix.EventType))
                        continue;

                    _events.Add(
                        $"{prefix.Affix.Name}{suffix.Affix.Name}",
                        complexType.MakeGenericType(prefix.EventType, suffix.EventType));
                }
            }

            var simpleType = typeof(CombatLogEvent<>);
            events.Where(i => i.IsSimple)
                .ToList()
                .ForEach(i => _events.Add(i.Affix.Name, simpleType.MakeGenericType(i.EventType)));
        }

        private static Dictionary<string, Type> _events = new Dictionary<string, Type>();

        public static CombatLogEvent GetCombatLogEvent(string line)
        {
            var @event = GetCombatLogEventType(line);
            return @event != null ? (CombatLogEvent)Activator.CreateInstance(@event, new object[] { line }) : null;
        }

        public static Type GetCombatLogEventType(string line)
        {
            var eventName = Regex.Match(line, @"^.*?\s{2}(?<eventName>.*?),.*").Groups["eventName"].Value;
            return _events.Where(i => i.Key == eventName).Select(i => i.Value).SingleOrDefault();
        }
    }
}

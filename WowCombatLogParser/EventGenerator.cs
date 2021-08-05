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
                .Select(i => new { AffixType = i.GetCustomAttribute<AffixAttribute>(), Type = i })
                .ToList();

            var prefixEvents = events.Where(i => i.AffixType is PrefixAttribute);
            var suffixEvents = events.Where(i => i.AffixType is SuffixAttribute);

            var complexType = typeof(CombatLogEvent<,>);
            foreach (var prefix in prefixEvents)
            {
                foreach (var suffix in suffixEvents)
                {
                    _events.Add(
                        $"{prefix.AffixType.Name}{suffix.AffixType.Name}",
                        complexType.MakeGenericType(prefix.Type, suffix.Type));
                }
            }

            var simpleType = typeof(CombatLogEvent<>);
            events.Where(i => !(i.AffixType is PrefixAttribute) && !(i.AffixType is SuffixAttribute))
                .ToList()
                .ForEach(i => _events.Add(i.AffixType.Name, simpleType.MakeGenericType(i.Type)));
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

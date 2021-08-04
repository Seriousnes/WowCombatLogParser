using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events
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

        public static IEnumerable<CombatLogEvent> GetCombatLogEvent(string line)            
        {
            var args = Regex.Replace(line, @"\s\s", ",").Split(',');
            var eventName = args[1];
            if (_events.ContainsKey(eventName))
            {
                yield return (CombatLogEvent)Activator.CreateInstance(_events[eventName], new object[] { args });
            }            
        }
    }
}

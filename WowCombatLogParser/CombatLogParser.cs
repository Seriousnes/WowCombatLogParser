using System;
using System.Collections.Generic;
using System.IO;
using WoWCombatLogParser.IO;

namespace WoWCombatLogParser
{
    public class CombatLogParser
    {
        private static readonly List<Type> _encounterEndEvents = new()
        {
            typeof(CombatLogEvent<EncounterEnd>),
            typeof(CombatLogEvent<ZoneChange>),
            typeof(CombatLogEvent<MapChange>)
        };

        public static IEnumerable<Encounter> ParseCombatLogSegments(string fileName)
        {
            List<CombatLogEvent> events = null;
            foreach (var @event in ParseCombatLog(fileName))
            {
                if (events != null)
                {
                    events.Add(@event);
                    if (_encounterEndEvents.Contains(@event.GetType()))
                    {
                        var segment = new Encounter();
                        segment.ProcessAsync(events).Wait();
                        events = null;
                        yield return segment;
                    }
                }
                else
                {
                    if (@event.GetType() == typeof(CombatLogEvent<EncounterStart>))
                    {
                        events = new();
                        events.Add(@event);
                    }
                }
            }
        }

        private static IEnumerable<CombatLogEvent> ParseCombatLog(string fileName)
        {
            foreach (var line in ReadCombatLog(fileName))
            {
                var combatLogEvent = EventGenerator.GetCombatLogEvent(GetConstructorParams(line));
                if (combatLogEvent != null)
                {
                    yield return combatLogEvent;
                }
            }
        }

        public static IList<IField> GetConstructorParams(string line)
        {
            using var s = new StringReader(line.Replace("  ", ","));
            using var r = new TextFieldReader(s) { Delimiters = new[] { ',' }, HasFieldsEnclosedInQuotes = true };
            return r.ReadFields();
        }

        private static IEnumerable<string> ReadCombatLog(string fileName)
        {
            using var sr = new StreamReader(fileName);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                yield return line;
            }
        }
    }
}

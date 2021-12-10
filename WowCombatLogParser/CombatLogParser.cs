using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using WoWCombatLogParser.Models;
using CsvHelper.Configuration;
using System;
using WoWCombatLogParser.Events.Simple;
using WoWCombatLogParser.Utilities;

namespace WoWCombatLogParser
{
    public class CombatLogParser
    {
        private static Regex preParser = new Regex(@"\s\s", RegexOptions.Compiled);
        private static List<Type> _encounterEndEvents = new()
        {
            typeof(CombatLogEvent<EncounterEnd>),
            typeof(CombatLogEvent<ZoneChange>),
            typeof(CombatLogEvent<MapChange>)
        };

        public static IEnumerable<CombatLogEvent> ParseCombatLog(string fileName)
        {
            foreach (var line in ReadCombatLog(fileName))
            {
                var combatLogEvent = EventGenerator.GetCombatLogEvent(PreProcess(line));
                if (combatLogEvent != null)
                {
                    yield return combatLogEvent;
                }
            }
        }

        public static IEnumerable<Segment> ParseCombatLogSegments(string fileName)
        {
            List<CombatLogEvent> events = null;
            foreach (var @event in ParseCombatLog(fileName))
            {
                if (events != null)
                {
                    events.Add(@event);
                    if (_encounterEndEvents.Contains(@event.GetType()))
                    {
                        var segment = new Segment();
                        segment.ParseSegment(events);
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

        private static IEnumerable<string> ReadCombatLog(string fileName)
        {
            using var sr = new StreamReader(fileName);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                yield return line;
            }
        }

        private static IEnumerable<string> PreProcess(string line)
        {
            return line
                .Replace("  ", ",")
                .Split(',');
        }
    }
}

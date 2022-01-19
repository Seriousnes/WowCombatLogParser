using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using WoWCombatLogParser.Models;
using CsvHelper.Configuration;
using System;
using WoWCombatLogParser.Events.Special;
using WoWCombatLogParser.Utility;
using Microsoft.VisualBasic.FileIO;
using System.Linq;

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

        public static IEnumerable<CombatLogEvent> ParseCombatLog(string fileName)
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
                        //segment.ParseSegmentAsync(events).Wait();
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

        public static IList<string> GetConstructorParams(string line)
        {
            using var s = new StringReader(line);
            using var r = new TextFieldParser(s) { Delimiters = new[] { ",", "  " }, HasFieldsEnclosedInQuotes = true };
            return r.ReadFields().ToList();
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

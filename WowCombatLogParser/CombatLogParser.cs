﻿using System;
using System.Collections.Generic;
using System.IO;
using WoWCombatLogParser.Common.Events;
using WoWCombatLogParser.IO;

namespace WoWCombatLogParser
{
    public interface ICombatLogParser
    {
        IEnumerable<Encounter> ParseCombatLogSegments(string fileName);
        IList<IField> GetConstructorParams(string line);
    }

    public class CombatLogParser : ICombatLogParser
    {
        private static readonly List<Type> _encounterEndEvents = new()
        {
            typeof(EncounterEnd),
            typeof(ZoneChange),
            typeof(MapChange)
        };

        public bool Async { get; set; } = false;

        public IEnumerable<Encounter> ParseCombatLogSegments(string fileName)
        {
            List<ICombatLogEvent> events = null;
            foreach (var @event in ParseCombatLog(fileName))
            {
                if (events != null)
                {
                    events.Add(@event);
                    if (_encounterEndEvents.Contains(@event.GetType()))
                    {
                        var segment = new Encounter();
                        if (Async)
                        {
                            segment.ProcessAsync(events).Wait();
                        }
                        else
                        {
                            segment.Process(events);
                        }
                        
                        events = null;
                        yield return segment;
                    }
                }
                else
                {
                    if (@event.GetType() == typeof(EncounterStart))
                    {
                        events = new();
                        events.Add(@event);
                    }
                }
            }
        }

        public IList<IField> GetConstructorParams(string line)
        {
            using var s = new StringReader(line.Replace("  ", ","));
            using var r = new TextFieldReader(s) { Delimiters = new[] { ',' }, HasFieldsEnclosedInQuotes = true };
            return r.ReadFields();
        }

        private IEnumerable<CombatLogEvent> ParseCombatLog(string fileName)
        {
            foreach (var line in ReadCombatLog(fileName))
            {
                var combatLogEvent = EventGenerator.GetCombatLogEvent<CombatLogEvent>(GetConstructorParams(line));
                if (combatLogEvent != null)
                {
                    yield return combatLogEvent;
                }
            }
        }
        
        private IEnumerable<string> ReadCombatLog(string fileName)
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

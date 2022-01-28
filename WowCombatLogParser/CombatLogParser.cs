using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser
{
    public interface ICombatLogParser
    {
        IEnumerable<Encounter> ParseCombatLogSegments(string fileName);
        Task<List<Encounter>> ParseCombatLogSegmentsAsync(string fileName);
    }

    public class CombatLogParser : ICombatLogParser
    {
        private static readonly List<Type> types = new()
        {
            typeof(ZoneChange),
            typeof(MapChange)
        };

        private static readonly Dictionary<Type, Type> _encounterEndEvents = new()
        {
            { typeof(EncounterStart), typeof(EncounterEnd) },
            { typeof(ArenaMatchStart), typeof(ArenaMatchEnd) },
            { typeof(ChallengeModeStart), typeof(ChallengeModeEnd) }
        };

        public IEnumerable<Encounter> ParseCombatLogSegments(string fileName)
        {
            List<ICombatLogEvent> events = null;
            foreach (var @event in ParseCombatLog(fileName))
            {
                if (events != null)
                {
                    events.Add(@event);
                    if (_encounterEndEvents.ContainsKey(@event.GetType()))
                    {
                        var segment = new Encounter();
                        segment.ProcessAsync(events).Wait();                        
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

        public async Task<List<Encounter>> ParseCombatLogSegmentsAsync(string fileName)
        {
            var events = await ParseCombatLogAsync(fileName);
            var results = new List<Encounter>();

            Encounter currentEncounter = null;
            foreach (var @event in events)
            {
                if (@event is IFightStart)
                {
                    currentEncounter = new Encounter();                    
                    results.Add(currentEncounter);
                }
                
                currentEncounter?.Events.Add(@event);                
               
                if (@event is IFightEnd)
                {
                    currentEncounter = null;
                }
            }            

            return null;
        }

        private IEnumerable<Models.CombatLogEvent> ParseCombatLog(string fileName)
        {
            foreach (var line in ReadCombatLog(fileName))
            {
                var combatLogEvent = line.GetCombatLogEvent<Models.CombatLogEvent>();
                if (combatLogEvent != null)
                {
                    combatLogEvent.Parse();
                    yield return combatLogEvent;
                }
            }
        }

        private async Task<List<Models.CombatLogEvent>> ParseCombatLogAsync(string fileName)
        {
            var stack = new ConcurrentBag<Models.CombatLogEvent>();
            await Parallel.ForEachAsync(ReadCombatLog(fileName), async (line, _) => stack.Add(await Task.Factory.StartNew(() => line.GetCombatLogEvent<Models.CombatLogEvent>())));
            return stack.ToList().OrderBy(x => x.Id).ToList();
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WoWCombatLogParser.Events.Parts;
using WoWCombatLogParser.Events.Special;
using WoWCombatLogParser.Utility;

namespace WoWCombatLogParser.Models
{
    [DebuggerDisplay("{GetEncounterDescription()}")]
    public class Encounter : List<CombatLogEvent>
    {
        public Encounter()
        {            
        }

        public void ParseSegment(IList<CombatLogEvent> events)
        {
            AddRange(events);
            Parse(); 
            Sort(_comparison);
        }

        public async Task ParseSegmentAsync(IList<CombatLogEvent> events)
        {
            AddRange(events);
            await ParseAsync();
            Sort(_comparison);
        }

        private void Parse()
        {
            ForEach(@event => @event.Parse());
        }

        private async Task ParseAsync()
        {
            await Parallel.ForEachAsync(this, async (@event, _) =>
            {
                await @event.ParseAsync();
            });
        }

        private Comparison<CombatLogEvent> _comparison => (c1, c2) => c1.Id.CompareTo(c2.Id);
        public IEnumerable<CombatantInfo> Combatants => this.OfType<CombatLogEvent<CombatantInfo>>().Select(x => x.Event);               

        public string GetEncounterDescription()
        {
            var start = this.First() is CombatLogEvent<EncounterStart> first ? first : new CombatLogEvent<EncounterStart>();
            var end = this.Last();
            
            // final event may not be an encounter end event
            var (success, duration) = end is CombatLogEvent<EncounterEnd> encounterEnd ? (encounterEnd.Event.Success, TimeSpan.FromMilliseconds(encounterEnd.Event.FightTime)) : (false, end.BaseEvent.Timestamp - start.BaseEvent.Timestamp);

            return $"{start.Event.Name} {start.Event.DifficultyId.GetDifficultyInfo().Name } {(success ? "Kill" : "Wipe" )} ({duration:m\\:ss})  {start.BaseEvent.Timestamp:h:mm tt}";
        }
    }
}

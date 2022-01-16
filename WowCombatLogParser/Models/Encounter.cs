using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WoWCombatLogParser.Events.Parts;

namespace WoWCombatLogParser.Models
{
    [DebuggerDisplay("Event Count: {Events.Count}")]
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


        public List<Ability> Abilities { get; set; } = new();
        //public List<Unit> 
    }
}

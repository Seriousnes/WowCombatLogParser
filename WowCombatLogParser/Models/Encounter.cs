using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            this.AddRange(events);
            Parse();
            Sort((c1, c2) => c1.Id.CompareTo(c2.Id));
        }

        private void Parse()
        {
            Task.WaitAll(this.Select(c => c.ParseAsync()).ToArray());        
        }
    }
}

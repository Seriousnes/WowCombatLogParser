using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WoWCombatLogParser.Models
{
    [DebuggerDisplay("Event Count: {Events.Count}")]
    public class Segment
    {
        public Segment()
        {
        }

        public List<CombatLogEvent> Events { get; private set; } = new();

        public void ParseSegment(IList<CombatLogEvent> events)
        {
            Events.AddRange(events);
            Parse();
            Events = Events.OrderBy(i => i.Id).ToList();            
        }

        private void Parse()
        {
            Task.WaitAll(Events.Select(c => c.ParseAsync()).ToArray());        
        }
    }
}

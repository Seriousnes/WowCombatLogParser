using System.Collections.Generic;
using System.Linq;
using WoWCombatLogParser.Events;
using static WoWCombatLogParser.Utilities.Extensions;

namespace WoWCombatLogParser.Models
{
    public abstract class CombatLogEvent : IEventSection
    {
        private IEnumerable<string> _line;
        private static int _count = 0;

        public CombatLogEvent()
        {
            Id = ++_count;
        }

        public CombatLogEvent(IEnumerable<string> line) : this()
        {
            _line = line;
        }       

        [NonData]
        public int Id { get; }
        [NonData]
        public bool HasBeenParsed { get; private set; } = false;
        public virtual EventBase BaseEvent { get; set; }
        public void DoParse()
        {
            if (HasBeenParsed) return;
            HasBeenParsed = true;
            var data = _line.GetEnumerator();
            this.Parse(data);            
            data.Dispose();
            _line = Enumerable.Empty<string>();                            
        }        
    }

    [DebuggerDisplay("{BaseEvent} {Event}")]
    public class CombatLogEvent<TEvent> : CombatLogEvent
        where TEvent : IEventSection, new()
    {
        public CombatLogEvent(IEnumerable<string> line) : base(line)
        {
            BaseEvent = new EventBase();            
        }

        public TEvent Event { get; } = new();
    }

    [DebuggerDisplay("{BaseEvent} {Prefix} {Suffix}")]
    public class CombatLogEvent<TPrefix, TSuffix> : CombatLogEvent
        where TPrefix : IEventSection, new()
        where TSuffix : IEventSection, new()
    {
        public CombatLogEvent(IEnumerable<string> line) : base(line)
        {
            BaseEvent = new ComplexEventBase();            
        }

        public TPrefix Prefix { get; } = new();
        public TSuffix Suffix { get; } = new();
    }
}
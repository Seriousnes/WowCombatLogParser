using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WoWCombatLogParser.Events;
using static WoWCombatLogParser.Utilities.Extensions;

namespace WoWCombatLogParser.Models
{
    public abstract class CombatLogEvent : IEventSection, ICombatLogEvent
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
        public abstract bool IsComplex { get; }
        public abstract bool IsOfType(Type type);
        public void Parse()
        {
            if (HasBeenParsed) return;
            HasBeenParsed = true;
            var data = _line.GetEnumerator();
            this.Parse(data);
            data.Dispose();
            _line = Enumerable.Empty<string>();
        }

        public Task ParseAsync()
        {
            return Task.Run(() => Parse());            
        }        
    }

    [DebuggerDisplay("{BaseEvent} {Event}")]
    public class CombatLogEvent<TEvent> : CombatLogEvent, ICombatLogEvent
        where TEvent : IEventSection, new()
    {
        public CombatLogEvent(IEnumerable<string> line) : base(line)
        {
            BaseEvent = new EventBase();            
        }

        public TEvent Event { get; } = new();

        public override bool IsComplex => false;
        public override bool IsOfType(Type type) => Event.GetType() == type || Event.GetType().IsSubclassOf(type);
    }

    [DebuggerDisplay("{BaseEvent} {Prefix} {Suffix}")]
    public class CombatLogEvent<TPrefix, TSuffix> : CombatLogEvent, ICombatLogEvent
        where TPrefix : IEventSection, new()
        where TSuffix : IEventSection, new()
    {
        public CombatLogEvent(IEnumerable<string> line) : base(line)
        {
            BaseEvent = new ComplexEventBase();            
        }

        public TPrefix Prefix { get; } = new();
        public TSuffix Suffix { get; } = new();

        public override bool IsComplex => true;
        public override bool IsOfType(Type type) =>
            Prefix.GetType() == type || Prefix.GetType().IsSubclassOf(type) ||
            Suffix.GetType() == type || Suffix.GetType().IsSubclassOf(type);
    }
}
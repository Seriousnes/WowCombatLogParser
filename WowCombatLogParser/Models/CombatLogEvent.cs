using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using WoWCombatLogParser.Events;
using WoWCombatLogParser.IO;

namespace WoWCombatLogParser.Models
{
    public abstract class CombatLogEvent : EventSection, ICombatLogEvent
    {
        private IEnumerable<IField> _line;
        private static int _count = 0;

        public CombatLogEvent()
        {
            Id = ++_count;
        }

        public CombatLogEvent(IEnumerable<IField> line = null, bool parseImmediate = false) : this()
        {
            _line = line;
            if (parseImmediate)
            {
                Parse();
            }
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
            var data = _line?.GetEnumerator();
            if (data?.MoveNext() ?? false)
            {
                Parse(data);
            }
            data?.Dispose();
            _line = null;
        }

        public Task ParseAsync()
        {
            return Task.Run(() => Parse());
        }

        public abstract string EventName { get; }
    }

    /// <summary>
    /// Container for events that have a single Affix component
    /// </summary>
    /// <remarks>
    /// Any class that inherits from this must reintroduced the following properties as "new", in this order: BaseEvent, Event. As the automatic property mapping builds the map in order of inheritance, if these
    /// properties are not reintroduced, the order would be mapped as Event, BaseEvent which is invalid for the parser
    /// </remarks>    
    [DebuggerDisplay("{BaseEvent} {Event}")]
    public class CombatLogEvent<TEvent> : CombatLogEvent, ICombatLogEvent
        where TEvent : EventSection, new()
    {
        public CombatLogEvent(IEnumerable<IField> line = null) : base(line)
        {
            BaseEvent = new EventBase();
        }

        public virtual TEvent Event { get; } = new();
        public override bool IsComplex => false;

        public override string EventName => Event.GetType().GetCustomAttribute<AffixAttribute>()?.Name;

        public override bool IsOfType(Type type) => Event.GetType() == type || Event.GetType().IsSubclassOf(type);
    }

    /// <summary>
    /// Container for compound events that have both a prefix and suffix
    /// </summary>
    /// <remarks>
    /// Any class that inherits from this must reintroduced the following properties as "new", in this order: BaseEvent, Prefix, Suffix. As the automatic property mapping builds the map in order of inheritance, if these
    /// properties are not reintroduced, the order would be mapped as Prefix, Suffix, BaseEvent which is invalid for the parser
    /// </remarks>    
    [DebuggerDisplay("{BaseEvent} {Prefix} {Suffix}")]
    public class CombatLogEvent<TPrefix, TSuffix> : CombatLogEvent, ICombatLogEvent
        where TPrefix : EventSection, new()
        where TSuffix : EventSection, new()
    {
        public CombatLogEvent(IEnumerable<IField> line = null) : base(line)
        {
            BaseEvent = new ComplexEventBase();
        }

        public virtual TPrefix Prefix { get; } = new();
        public virtual TSuffix Suffix { get; } = new();
        public override bool IsComplex => true;

        public override string EventName => $"{Prefix.GetType().GetCustomAttribute<AffixAttribute>().Name}{Suffix.GetType().GetCustomAttribute<AffixAttribute>().Name}";

        public override bool IsOfType(Type type) =>
            Prefix.GetType() == type || Prefix.GetType().IsSubclassOf(type) ||
            Suffix.GetType() == type || Suffix.GetType().IsSubclassOf(type);
    }
}
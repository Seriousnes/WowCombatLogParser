using System;

namespace WoWCombatLogParser.Events
{
    public interface ICombatLogEvent
    {
        bool IsComplex { get; }
        bool IsOfType(Type type);
    }

    [DebuggerDisplay("{Timestamp} {Event}")]
    public class EventBase : EventSection
    {
        public DateTime Timestamp { get; set; }
        public string Event { get; set; }
    }

    [DebuggerDisplay("{Timestamp} {Event} {Source} {Destination}")]
    public class ComplexEventBase : EventBase
    {
        public Unit Source { get; } = new();
        public Unit Destination { get; } = new();
    }
}

using System;
using System.Diagnostics;
using WoWCombatLogParser.Events.Parts;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events
{
    public interface IEventSection
    {
    }

    public class EventBase : IEventSection
    {        
        public DateTime Timestamp { get; set; }
        public string Event { get; set; }
    }

    [DebuggerDisplay("{Event} @ {Timestamp} {Source} > {Destination}")]
    public class ComplexEventBase : EventBase
    {
        public Unit Source { get; } = new();
        public Unit Destination { get; } = new();
    }
}

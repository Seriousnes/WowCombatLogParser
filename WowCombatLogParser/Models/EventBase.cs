using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using WoWCombatLogParser.Events.Parts;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events
{
    public interface IEventSection
    {
    }

    [DebuggerDisplay("{Timestamp} {Event}")]
    public class EventBase : IEventSection
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

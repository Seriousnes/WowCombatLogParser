using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using WoWCombatLogParser.Events.Parts;
using WoWCombatLogParser.Models;
using WoWCombatLogParser.Utilities;

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
        public Actor Source { get; } = new();
        public Actor Destination { get; } = new();        
    }
}

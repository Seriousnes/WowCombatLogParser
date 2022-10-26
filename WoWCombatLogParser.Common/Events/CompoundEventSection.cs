using System;
using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    [DebuggerDisplay("{Timestamp} {Event} {Source} {Destination}")]
    public partial class CompoundEventSection : EventSection
    {
        public Unit Source { get; set; } = new();
        public Unit Destination { get; set; } = new();
    }
}

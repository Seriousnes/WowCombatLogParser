using System;
using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    [DebuggerDisplay("{Timestamp} {Event} {Source} {Destination}")]
    public partial class CompoundEventSection : EventSection
    {
        public Unit Source { get; } = new Unit();
        public Unit Destination { get; } = new Unit();
    }
}

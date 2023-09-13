using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

[DebuggerDisplay("{Timestamp} {CombatLogEventComponent} {Source} {Destination}")]
public partial class CompoundEventSection : CombatLogEventComponent
{
    public Unit Source { get; set; } = new();
    public Unit Destination { get; set; } = new();
}

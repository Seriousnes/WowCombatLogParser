using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

[DebuggerDisplay("{Timestamp} {CombagLogEventComponent} {Source} {Destination}")]
public partial class CompoundEventSection : CombagLogEventComponent
{
    public Unit Source { get; set; } = new();
    public Unit Destination { get; set; } = new();
}

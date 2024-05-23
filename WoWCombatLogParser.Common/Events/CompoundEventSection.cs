using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

[DebuggerDisplay("{Timestamp} {CombatLogEventComponent} {Source} {Destination}")]
public partial class CompoundEventSection : CombatLogEventComponent
{
    public Actor Source { get; set; } = new();
    public Actor Destination { get; set; } = new();
}

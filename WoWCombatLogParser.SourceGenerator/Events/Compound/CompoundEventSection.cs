using System.Diagnostics;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound;

[DebuggerDisplay("{Timestamp} {CombatLogEventComponent} {Source} {Destination}")]
internal partial class CompoundEventSection : CombatLogEventComponent
{
    public Actor Source { get; set; } = new();
    public Actor Destination { get; set; } = new();
}

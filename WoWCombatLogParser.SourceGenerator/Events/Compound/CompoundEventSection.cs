namespace WoWCombatLogParser.SourceGenerator.Events.Compound;

[DebuggerDisplay("{Timestamp} {CombatLogEventComponent} {Source} {Destination}")]
internal partial class CompoundEventSection : CombatLogEventComponent, IHasSource, IHasDestination
{
    public Actor Source { get; set; } = new();
    public Actor Destination { get; set; } = new();
}

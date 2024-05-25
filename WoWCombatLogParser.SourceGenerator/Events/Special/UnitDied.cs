using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Special;

[Affix("UNIT_DIED")]
internal class UnitDied
{
    public WowGuid RecapId { get; set; }
    public bool UnconsciousOnDeath { get; set; }
    public object UnusedField1 { get; set; }
    public object UnusedField2 { get; set; }
    public Actor Unit { get; set; } = new Actor();
}

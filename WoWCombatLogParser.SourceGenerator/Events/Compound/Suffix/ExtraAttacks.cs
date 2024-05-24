using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;

[Suffix("_EXTRA_ATTACKS")]
internal class ExtraAttacks : CombatLogEventComponent
{
    public int Amount { get; set; }
}

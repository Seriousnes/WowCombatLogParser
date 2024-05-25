using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;

[Suffix("_AURA_APPLIED_DOSE")]
[DebuggerDisplay("{AuraType} {Stacks}")]
internal class AuraAppliedDose : CombatLogEventComponent, IAura
{
    public AuraType AuraType { get; set; }
    public decimal Stacks { get; set; }
}

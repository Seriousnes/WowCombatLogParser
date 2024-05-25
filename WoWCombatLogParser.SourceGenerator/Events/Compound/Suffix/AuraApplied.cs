using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;

[Suffix("_AURA_APPLIED")]
[DebuggerDisplay("{AuraType} {Amount}")]
internal class AuraApplied : CombatLogEventComponent, IAura
{
    public AuraType AuraType { get; set; }
    [Optional]
    public decimal Amount { get; set; }
}

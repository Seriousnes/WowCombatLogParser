using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;

[Suffix("_AURA_BROKEN")]
internal class AuraBroken : CombatLogEventComponent, IAura
{
    public AuraType AuraType { get; set; }
}

[Suffix("_AURA_BROKEN_SPELL")]
internal class AuraBrokenSpell : SuffixAbilityBase, IAura
{
    public AuraType AuraType { get; set; }
}

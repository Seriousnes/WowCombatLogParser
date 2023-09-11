using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

[Suffix("_AURA_BROKEN")]
public class AuraBroken : CombagLogEventComponent, IAura
{
    public AuraType AuraType { get; set; }
}

[Suffix("_AURA_BROKEN_SPELL")]
public class AuraBrokenSpell : SuffixAbilityBase, IAura
{
    public AuraType AuraType { get; set; }
}

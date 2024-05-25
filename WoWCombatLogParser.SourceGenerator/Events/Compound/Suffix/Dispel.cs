using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;

[Suffix("_DISPEL")]
internal class Dispel : SuffixAbilityBase, IAura
{
    public AuraType AuraType { get; set; }
}

[Suffix("_STOLEN")]
internal class Stolen : SuffixAbilityBase, IAura
{
    public AuraType AuraType { get; set; }
}

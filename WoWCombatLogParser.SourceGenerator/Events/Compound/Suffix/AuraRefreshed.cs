using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;

[Suffix("_AURA_REFRESH")]
internal class AuraRefreshed : CombatLogEventComponent, IAura
{
    public AuraType AuraType { get; set; }
}

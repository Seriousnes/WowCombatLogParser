using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

[Suffix("_AURA_REFRESH")]
public class AuraRefreshed : CombatLogEventComponent, IAura
{
    public AuraType AuraType { get; set; }
}

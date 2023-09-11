using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

[Suffix("_AURA_REFRESH")]
public class AuraRefreshed : CombagLogEventComponent, IAura
{
    public AuraType AuraType { get; set; }
}

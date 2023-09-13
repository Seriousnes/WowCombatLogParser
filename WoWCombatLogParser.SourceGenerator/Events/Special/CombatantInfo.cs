using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

public abstract class CombatantInfo : CombatLogEventComponent
{
    public WowGuid PlayerGuid { get; set; }
    public Faction Faction { get; set; }
}

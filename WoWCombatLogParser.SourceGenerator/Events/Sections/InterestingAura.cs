using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

public class InterestingAura : CombagLogEventComponent
{
    public WowGuid PlayerId { get; set; }
    public int AuraId { get; set; }
}

namespace WoWCombatLogParser.Models;

public class InterestingAura : CombatLogEventComponent
{
    public WowGuid PlayerId { get; set; }
    public int AuraId { get; set; }
}

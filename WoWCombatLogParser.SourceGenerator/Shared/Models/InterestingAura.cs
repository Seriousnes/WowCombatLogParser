namespace WoWCombatLogParser;

public class InterestingAura : CombatLogEventComponent
{
    public WowGuid PlayerId { get; set; }
    public int AuraId { get; set; }
}

using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Sections;

internal class InterestingAura : CombatLogEventComponent
{
    public WowGuid PlayerId { get; set; }
    public int AuraId { get; set; }
}

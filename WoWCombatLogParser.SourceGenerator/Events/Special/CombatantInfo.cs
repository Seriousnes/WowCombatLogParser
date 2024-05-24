using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Special;

internal abstract class CombatantInfo : CombatLogEventComponent
{
    public WowGuid PlayerGuid { get; set; }
    public Faction Faction { get; set; }
}

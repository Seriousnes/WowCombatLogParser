using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Special;

[Affix("COMBAT_LOG_VERSION")]
internal class CombatLogVersionEvent
{
    public CombatLogVersion Version { get; set; }
    public bool AdvancedLogEnabled { get; set; }
    public string BuildVersion { get; set; }
    public int ProjectId { get; set; }
}

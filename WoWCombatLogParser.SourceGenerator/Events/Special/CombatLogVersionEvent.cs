using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

[Affix("COMBAT_LOG_VERSION")]
public class CombatLogVersionEvent : BaseCombatLogEvent
{    
    public CombatLogVersion Version { get; set; }
    public bool AdvancedLogEnabled { get; set; }
    public string BuildVersion { get; set; }
    public int ProjectId { get; set; }    
}

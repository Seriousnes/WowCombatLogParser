using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

[Suffix("_AURA_REMOVED")]
[DebuggerDisplay("{AuraType}")]
public class AuraRemoved : CombatLogEventComponent, IAura
{
    public AuraType AuraType { get; set; }
    [Optional]
    public int Amount { get; set; }
}

using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

[Suffix("_AURA_REMOVED")]
[DebuggerDisplay("{AuraType} {Amount}")]
public class AuraRemoved : Event, IAura
{
    public AuraType AuraType { get; set; }
    public decimal Amount { get; set; }
}

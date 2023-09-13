using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

[Suffix("_AURA_APPLIED")]
[DebuggerDisplay("{AuraType} {Amount}")]
public class AuraApplied : CombatLogEventComponent, IAura
{
    public AuraType AuraType { get; set; }
    public decimal Amount { get; set; }
}

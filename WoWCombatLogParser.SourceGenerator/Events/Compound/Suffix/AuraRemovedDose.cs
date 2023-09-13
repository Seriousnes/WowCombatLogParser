using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

[Suffix("_AURA_REMOVED_DOSE")]
[DebuggerDisplay("{AuraType} {Stacks}")]
public class AuraRemovedDose : CombatLogEventComponent, IAura
{
    public AuraType AuraType { get; set; }
    public decimal Stacks { get; set; }
}

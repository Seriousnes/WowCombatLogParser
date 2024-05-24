using System.Diagnostics;
using WoWCombatLogParser.Models;
using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;

[Suffix("_AURA_REMOVED")]
[DebuggerDisplay("{AuraType}")]
internal class AuraRemoved : CombatLogEventComponent, IAura
{
    public AuraType AuraType { get; set; }
    [Optional]
    public int Amount { get; set; }
}

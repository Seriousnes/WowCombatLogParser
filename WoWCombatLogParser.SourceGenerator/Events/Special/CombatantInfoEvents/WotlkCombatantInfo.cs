using System.Diagnostics;
using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Special.CombatantInfoEvents;

[CombatLogVersion(CombatLogVersion.Wotlk)]
[Affix("COMBATANT_INFO")]
[DebuggerDisplay("{PlayerGuid} {Faction}")]
internal class WotlkCombatantInfo : CombatantInfo, ICombatantInfo
{
}

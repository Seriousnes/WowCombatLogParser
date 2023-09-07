using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

[CombatLogVersion(CombatLogVersion.Wotlk)]
[Affix("COMBATANT_INFO")]
[DebuggerDisplay("{PlayerGuid} {Faction}")]
public class WotlkCombatantInfo : CombatantInfo, ICombatantInfo
{
}

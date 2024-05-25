using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Special.CombatantInfoEvents;

[CombatLogVersion(CombatLogVersion.Wotlk)]
[Affix("COMBATANT_INFO")]
[DebuggerDisplay("{PlayerGuid} {Faction}")]
internal abstract class WotlkCombatantInfo : CombatantInfo, ICombatantInfo
{
}

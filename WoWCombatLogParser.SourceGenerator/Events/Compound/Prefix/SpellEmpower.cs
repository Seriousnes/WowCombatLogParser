using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

[Prefix("SPELL_EMPOWER")]
[SuffixAllowed(typeof(Start), typeof(End))]
public class SpellEmpower : Spell
{
}
using WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;
using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Prefix;

[Prefix("SPELL_EMPOWER")]
[SuffixAllowed(typeof(Start), typeof(End))]
internal class SpellEmpower : Spell
{
}
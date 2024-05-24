using WoWCombatLogParser.SourceGenerator.Events.Compound.Prefix;
using WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;
using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Predefined;

[Affix("SPELL_ABSORBED")]
internal class SpellAbsorbed : Predefined<Spell, Absorbed>
{
}

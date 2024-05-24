using WoWCombatLogParser.SourceGenerator.Events.Compound.Prefix;
using WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;
using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Predefined;

[Affix("DAMAGE_SHIELD")]
internal class DamageShield : Predefined<Spell, Damage>
{
}

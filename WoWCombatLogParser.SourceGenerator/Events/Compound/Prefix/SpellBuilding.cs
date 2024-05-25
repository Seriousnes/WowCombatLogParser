using WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;
using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Prefix;

[Prefix("SPELL_BUILDING")]
[SuffixAllowed(typeof(Damage), typeof(Healing))]
internal class SpellBuilding : PrefixAbilityBase, IAbility
{
}

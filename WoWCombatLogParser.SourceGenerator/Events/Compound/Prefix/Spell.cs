using WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;
using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Prefix;

[Prefix("SPELL")]
[SuffixNotAllowed(typeof(DamageLanded))]
internal class Spell : PrefixAbilityBase, IAbility
{
}
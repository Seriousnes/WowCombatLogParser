using WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;
using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Prefix;

[Prefix("RANGE")]
[SuffixAllowed(typeof(Damage), typeof(Missed))]
[SuffixNotAllowed(typeof(DamageLanded))]
internal class Range : PrefixAbilityBase, IAbility
{
}

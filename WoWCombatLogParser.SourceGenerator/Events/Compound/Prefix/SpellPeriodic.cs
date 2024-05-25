using WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;
using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Prefix;

[Prefix("SPELL_PERIODIC")]
[SuffixAllowed(typeof(Healing), typeof(Damage), typeof(Energize), typeof(Missed))]
internal class SpellPeriodic : PrefixAbilityBase
{
}

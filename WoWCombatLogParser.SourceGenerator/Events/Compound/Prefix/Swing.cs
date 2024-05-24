using WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;
using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Prefix;

[Prefix("SWING")]
[SuffixAllowed(typeof(Damage), typeof(DamageLanded), typeof(Missed))]
internal class Swing : CombatLogEventComponent
{
}

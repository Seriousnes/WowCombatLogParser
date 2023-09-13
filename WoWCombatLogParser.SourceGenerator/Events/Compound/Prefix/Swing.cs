using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

[Prefix("SWING")]
[SuffixAllowed(typeof(Damage), typeof(DamageLanded), typeof(Missed))]
public class Swing : CombatLogEventComponent
{
}

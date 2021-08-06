using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex
{
    [Prefix("RANGE")]
    [SuffixAllowed(typeof(Damage), typeof(Missed))]
    public class Range : SpellBase
    {
    }
}

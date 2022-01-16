using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex
{
    [Prefix("SWING")]
    [SuffixAllowed(typeof(Damage), typeof(Missed))]
    public class Swing : Part
    {
    }
}

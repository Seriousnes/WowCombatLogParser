using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex
{
    [Prefix("SPELL_PERIODIC")]
    [SuffixAllowed(typeof(Healing), typeof(Damage), typeof(Energize), typeof(Missed))]
    public class SpellPeriodic : SpellBase
    {
    }
}

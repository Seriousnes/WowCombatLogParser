using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex
{
    [Prefix("SPELL_BUILDING")]
    [SuffixAllowed(typeof(Damage), typeof(Healing))]
    public class SpellBuilding : SpellBase
    {
    }
}

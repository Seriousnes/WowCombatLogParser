using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    [Affix("DAMAGE_SHIELD_MISSED")]
    public class DamageShieldMissed : Predefined<Spell, Missed>
    {
    }
}

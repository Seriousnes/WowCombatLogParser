using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    [Affix("DAMAGE_SPLIT")]
    public class DamageSplit : Predefined<Spell, Damage>
    {
    }
}

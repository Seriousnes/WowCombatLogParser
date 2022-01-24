using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    [Prefix("SPELL_BUILDING")]
    [SuffixAllowed(typeof(Damage), typeof(Healing))]
    public class SpellBuilding : PrefixAbilityBase
    {
    }
}

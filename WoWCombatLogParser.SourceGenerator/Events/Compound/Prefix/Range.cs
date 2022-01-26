using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    [Prefix("RANGE")]
    [SuffixAllowed(typeof(Damage), typeof(Missed))]
    [SuffixNotAllowed(typeof(DamageLanded))]
    public class Range : PrefixAbilityBase, IAbility
    {        
    }
}

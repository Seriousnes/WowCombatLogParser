using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

[Prefix("SPELL")]
[SuffixNotAllowed(typeof(DamageLanded))]
public class Spell : PrefixAbilityBase, IAbility
{        
}
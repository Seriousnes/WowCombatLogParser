using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

[Prefix("SPELL")]
[SuffixNotAllowed(typeof(DamageLanded))]
public class Spell : PrefixAbilityBase, IAbility
{        
}

[Prefix("SPELL_EMPOWER")]
[SuffixAllowed(typeof(Start), typeof(End), typeof(Interrupt))]
public class SpellEmpower : Spell
{
}
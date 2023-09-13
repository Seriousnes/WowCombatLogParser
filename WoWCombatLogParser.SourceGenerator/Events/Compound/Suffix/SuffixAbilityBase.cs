using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

public abstract class SuffixAbilityBase : CombatLogEventComponent
{
    public Ability ExtraSpell { get; set; } = new();
}

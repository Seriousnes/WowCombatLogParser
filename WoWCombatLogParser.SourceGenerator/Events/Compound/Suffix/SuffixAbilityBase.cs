using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

public abstract class SuffixAbilityBase : Event
{
    public Ability ExtraSpell { get; set; } = new();
}

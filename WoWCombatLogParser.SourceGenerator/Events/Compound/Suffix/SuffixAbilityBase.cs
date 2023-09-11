using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

public abstract class SuffixAbilityBase : CombagLogEventComponent
{
    public Ability ExtraSpell { get; set; } = new();
}

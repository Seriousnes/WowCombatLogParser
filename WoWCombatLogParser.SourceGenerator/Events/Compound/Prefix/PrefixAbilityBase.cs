using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

public abstract class PrefixAbilityBase : Event
{
    public Ability Spell { get; set; } = new();
}

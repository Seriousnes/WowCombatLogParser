using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

public abstract class PrefixAbilityBase : CombatLogEventComponent
{
    public Ability Spell { get; set; } = new();
}

using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

public abstract class PrefixAbilityBase : CombagLogEventComponent
{
    public Ability Spell { get; set; } = new();
}

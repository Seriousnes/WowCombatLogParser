using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Prefix;

internal abstract class PrefixAbilityBase : CombatLogEventComponent
{
    public Ability Spell { get; set; } = new();
}

using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;

internal abstract class SuffixAbilityBase : CombatLogEventComponent
{
    public Ability ExtraSpell { get; set; } = new();
}

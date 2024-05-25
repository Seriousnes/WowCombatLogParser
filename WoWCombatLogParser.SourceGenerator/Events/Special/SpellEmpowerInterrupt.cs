using WoWCombatLogParser.SourceGenerator.Events.Compound;
using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Special;

[Affix("SPELL_EMPOWER_INTERRUPT")]
internal class SpellEmpowerInterrupt : CompoundEventSection, IEmpowerFinish
{
    public Ability Spell { get; set; }
    public int Stage { get; set; }
}

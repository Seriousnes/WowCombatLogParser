using System.Diagnostics;
using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;

[Suffix("_HEAL_ABSORBED")]
[DebuggerDisplay("{ExtraUnit} {ExtraSpell} {ExtraAmount}")]
internal class HealingAbsorbed
{
    public Unit ExtraUnit { get; set; } = new Unit();
    public Ability ExtraSpell { get; set; } = new Ability();
    public int ExtraAmount { get; set; }
}

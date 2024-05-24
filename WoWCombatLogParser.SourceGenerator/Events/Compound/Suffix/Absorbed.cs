using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;

internal class Absorbed
{
    public Actor AbsorbedBy { get; set; }
    public Ability ExtraSpell { get; set; }
    public int AbsorbedAmount { get; set; }
    public decimal UnmitigatedAmount { get; set; }
}

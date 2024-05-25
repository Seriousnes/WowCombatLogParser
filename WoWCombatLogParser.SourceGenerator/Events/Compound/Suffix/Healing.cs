using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;

[Suffix("_HEAL")]
[DebuggerDisplay("{Amount} {Overhealing} {Absorbed} {Critical}")]
internal class Healing : AdvancedLoggingDetailsBase, IHealing, IDamageOrHealing, IAdvancedLoggingDetails
{
    public int Overhealing { get; set; }
    public int Absorbed { get; set; }
    public bool Critical { get; set; }
}

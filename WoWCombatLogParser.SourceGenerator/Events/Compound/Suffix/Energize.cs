using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;

[Suffix("_ENERGIZE")]
[DebuggerDisplay("{OverEnergize} {PowerType} {AlternatePowerType}")]
internal class Energize : AdvancedLoggingDetailsBase, IAdvancedLoggingDetails
{
    public decimal OverEnergize { get; set; }
    public PowerType PowerType { get; set; }
    public PowerType AlternatePowerType { get; set; }
}

using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;

[Suffix("_DRAIN")]
[DebuggerDisplay("{OverEnergize} {PowerType} {AlternatePowerType}")]
internal class Drain : AdvancedLoggingDetailsBase, IDrain
{
    public PowerType PowerType { get; set; }
    public decimal ExtraAmount { get; set; }
    public decimal MaxPower { get; set; }
}

[Suffix("_LEECH")]
[DebuggerDisplay("{OverEnergize} {PowerType} {AlternatePowerType}")]
internal class Leech : AdvancedLoggingDetailsBase, IDrain
{
    public PowerType PowerType { get; set; }
    public decimal ExtraAmount { get; set; }
}

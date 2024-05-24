using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;

[Suffix("_START")]
internal class Start : CombatLogEventComponent
{
}

[Suffix("_END")]
internal class End : CombatLogEventComponent, IEmpowerFinish
{
    public int Stage { get; set; }
}

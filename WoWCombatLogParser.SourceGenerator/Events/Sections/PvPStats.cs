namespace WoWCombatLogParser.SourceGenerator.Events.Sections;

internal class PvPStats : CombatLogEventComponent
{
    public int HonorLevel { get; set; }
    public int Season { get; set; }
    public int Rating { get; set; }
    public int Tier { get; set; }
}

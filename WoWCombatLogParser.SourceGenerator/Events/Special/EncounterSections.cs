using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Special;

internal abstract class EncounterFragment : CombatLogEventComponent
{
    public int EncounterId { get; set; }
    public string Name { get; set; }
    public Difficulty DifficultyId { get; set; }
    public int GroupSize { get; set; }
}


[DebuggerDisplay("Encounter \"{Name}\" ({Id}) starting")]
[Affix("ENCOUNTER_START")]
internal class EncounterStart : EncounterFragment, IFightStart
{
    public int InstanceId { get; set; }
}

[DebuggerDisplay("Encounter \"{Name}\" ({Id}) ended")]
[Affix("ENCOUNTER_END")]
internal class EncounterEnd : EncounterFragment, IFightEnd, IFightEndSuccess
{
    public bool Success { get; set; }
    public int Duration { get; set; }
}


internal abstract class LocationChange : CombatLogEventComponent
{
    public int LocationId { get; set; }
    public string Name { get; set; }
}

[DebuggerDisplay("Loaded into map \"{Name}\" ({Id})")]
[Affix("MAP_CHANGE")]
internal class MapChange : LocationChange
{
}

[DebuggerDisplay("Loaded into zone \"{Name}\" ({Id})")]
[Affix("ZONE_CHANGE")]
internal class ZoneChange : LocationChange
{
    public int DifficultyId { get; set; }
}

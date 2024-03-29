﻿using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

public abstract class EncounterFragment : CombatLogEventComponent
{
    public int EncounterId { get; set; }
    public string Name { get; set; }
    public Difficulty DifficultyId { get; set; }
    public int GroupSize { get; set; }
}


[DebuggerDisplay("Encounter \"{Name}\" ({Id}) starting")]
[Affix("ENCOUNTER_START")]
public class EncounterStart : EncounterFragment, IFightStart
{
    public int InstanceId { get; set; }
}

[DebuggerDisplay("Encounter \"{Name}\" ({Id}) ended")]
[Affix("ENCOUNTER_END")]
public class EncounterEnd : EncounterFragment, IFightEnd, IFightEndSuccess
{
    public bool Success { get; set; }
    public int Duration { get; set; }
}


public abstract class LocationChange : CombatLogEventComponent
{
    public int LocationId { get; set; }
    public string Name { get; set; }
}

[DebuggerDisplay("Loaded into map \"{Name}\" ({Id})")]
[Affix("MAP_CHANGE")]
public class MapChange : LocationChange
{
}

[DebuggerDisplay("Loaded into zone \"{Name}\" ({Id})")]
[Affix("ZONE_CHANGE")]
public class ZoneChange : LocationChange
{
    public int DifficultyId { get; set; }
}

using System;
using System.Collections.Generic;
using System.Text;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Special;

[Affix("WORLD_MARKER_PLACED")]
public class WorldMarkerPlaced
{
    public int MapId { get; set; }
    public WorldMarker Marker { get; set; }
    public decimal X { get; set; }  
    public decimal Y { get; set; }
}

[Affix("WORLD_MARKER_REMOVED")]
public class WorldMarkerRemoved
{
    public WorldMarker Marker { get; set; }
}
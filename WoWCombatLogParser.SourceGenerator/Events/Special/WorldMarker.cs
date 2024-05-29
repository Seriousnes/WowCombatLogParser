using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Special;

[Affix("WORLD_MARKER_PLACED")]
internal class WorldMarkerPlaced
{
    public int MapId { get; set; }
    public WorldMarker Marker { get; set; }
    public decimal X { get; set; }
    public decimal Y { get; set; }
}

[Affix("WORLD_MARKER_REMOVED")]
internal class WorldMarkerRemoved
{
    public WorldMarker Marker { get; set; }
}
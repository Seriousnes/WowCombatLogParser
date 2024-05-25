using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Special;

[Affix("EMOTE")]
internal class Emote
{
    public Unit Source { get; set; } = new();
    public Unit Destination { get; set; } = new();
    public string Text { get; set; }
}

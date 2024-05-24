using WoWCombatLogParser.Models;
using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Special;

[Affix("ARENA_MATCH_START")]
internal class ArenaMatchStart : CombatLogEventComponent, IFightStart
{
    public int InstanceId { get; set; }
    public object UnknownProperty1 { get; set; }
    public MatchType MatchType { get; set; }
    public int TeamId { get; set; }
}

[Affix("ARENA_MATCH_END")]
internal class ArenaMatchEnd : CombatLogEventComponent, IFightEnd
{
    public int WinningTeam { get; set; }
    public int Duration { get; set; }
    public int NewRatingTeam1 { get; set; }
    public int NewRatingTeam2 { get; set; }
}

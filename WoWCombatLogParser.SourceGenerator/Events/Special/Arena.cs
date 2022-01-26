using System;
using System.Collections.Generic;
using System.Text;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    [Affix("ARENA_MATCH_START")]
    public class ArenaMatchStart : EventSection
    {
        public int InstanceId { get; set; }
        public object UnknownProperty1 { get; set; }
        public MatchType MatchType { get; set; }
        public int TeamId { get; set; }
    }

    [Affix("ARENA_MATCH_END")]
    public class ArenaMatchEnd : EventSection, IEncounterEnd
    {
        public int WinningTeam { get; set; }
        public int Duration { get; set; }
        public int NewRatingTeam1 { get; set; }
        public int NewRatingTeam2 { get; set; }
    }
}

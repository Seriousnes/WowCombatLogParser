using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Models
{
    [DebuggerDisplay("{GetDescription()}")]
    public abstract class MutableFight<TStart, TEnd> : Fight<TStart, TEnd>
        where TStart : CombatLogEvent, IFightStart
        where TEnd : CombatLogEvent, IFightEnd
    {
        protected MutableFight(TStart start) : base(start)
        {
        }

        public override CombatLogEvent AddEvent(CombatLogEvent @event)
        {
            _events.Add(@event);
            @event.Encounter = this;
            if (@event is TEnd endEvent)
            {
                _end = endEvent;
                _end.ParseAsync().Wait();
            }
            return @event;
        }

        public override async Task ParseAsync()
        {
            await Parallel.ForEachAsync(_events, async (e, _) => await e.ParseAsync());
        }
    }

    public class Raid : MutableFight<EncounterStart, EncounterEnd>
    {
        public Raid(EncounterStart start) : base(start)
        {
        }

        public override string Name => _start.Name;
        public override string Result => _end is EncounterEnd endOfFight && endOfFight.Success ? "Kill" : "Wipe";
    }

    public class ChallengeMode : MutableFight<ChallengeModeStart, ChallengeModeEnd>
    {
        public ChallengeMode(ChallengeModeStart start) : base(start)
        {
        }

        public override string Name => $"{_start.InstanceId} Level {_start.KeystoneLevel} (Affixes: {string.Join(',', _start.Affixes?.Select(x => x.Id.ToString()))})";
        public override string Result => _end is ChallengeModeEnd endOfFight && endOfFight.Success ? "Timed" : "Not timed";
    }

    public class ArenaMatch : MutableFight<ArenaMatchStart, ArenaMatchEnd>
    {
        public ArenaMatch(ArenaMatchStart start) : base(start)
        {
        }

        public override string Name => _start.InstanceId.ToString();
        public override string Result => _end is ArenaMatchEnd endOfFight ? $"Team {endOfFight.WinningTeam} wins. New ratings: Team1 = {endOfFight.NewRatingTeam1}, Team2 = {endOfFight.NewRatingTeam2}" : "";
    }
}

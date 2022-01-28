using System;
using System.Linq;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Models
{
    [DebuggerDisplay("{GetDescription()}")]
    public abstract class Fight<TStart, TEnd> 
        where TStart : CombatLogEvent, IFightStart
        where TEnd : CombatLogEvent, IFightEnd
    {
        protected TStart _start;
        protected TEnd _end;
        protected List<CombatLogEvent> _events = new();

        public Fight(TStart start)
        {
            _start = start;
            _events.Add(start);
        }

        public TimeSpan Duration => _end is null ? (_events.Last().Timestamp - _start.Timestamp) : TimeSpan.FromMilliseconds(_end.Duration);
        public CombatLogEvent AddEvent(CombatLogEvent @event)
        {
            _events.Add(@event);
            if (_events is TEnd endEvent)
            {
                _end = endEvent;
                _events = _events.OrderBy(x => x.Id).ToList();
            }
            return @event;
        }

        public virtual FightDescription GetDescription() => new(Name, Duration, _start.Timestamp, Result);
        public abstract string Name { get; }
        public abstract string Result { get; }
    }

    [DebuggerDisplay("{Description} ({Result}) {Duration} {Time}")]
    public class FightDescription
    {
        public FightDescription(string description, TimeSpan duration, DateTime time, string result)
        {
            Description = description;
            Duration = $"{duration:m\\:ss}";
            Time = $"{time:h:mm tt}";
            Result = result;
        }

        public string Description { get; set; }
        public string Duration { get; set; }
        public string Result { get; set; }
        public string Time { get; set; }
    }

    public class Raid : Fight<EncounterStart, EncounterEnd>
    {
        public Raid(EncounterStart start) : base(start)
        {
        }

        public override string Name => _start.Name;
        public override string Result => _end is EncounterEnd endOfFight && endOfFight.Success ? "Kill" : "Wipe";
    }

    public class ChallengeMode : Fight<ChallengeModeStart, ChallengeModeEnd>
    {
        public ChallengeMode(ChallengeModeStart start) : base(start)
        {
        }

        public override string Name => $"{_start.InstanceId} Level {_start.KeystoneLevel} (Affixes: {string.Join(',', _start.Affixes?.Select(x => x.Id.ToString()))})";
        public override string Result => _end is ChallengeModeEnd endOfFight && endOfFight.Success ? "Timed" : "Not timed";
    }

    public class Arena : Fight<ArenaMatchStart, ArenaMatchEnd>
    {
        public Arena(ArenaMatchStart start) : base(start)
        {
        }

        public override string Name => _start.InstanceId.ToString();
        public override string Result => _end is ArenaMatchEnd endOfFight ? $"Team {endOfFight.WinningTeam} wins. New ratings: Team1 = {endOfFight.NewRatingTeam1}, Team2 = {endOfFight.NewRatingTeam2}" : "";
    }
}

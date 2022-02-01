using System;
using System.Linq;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Models
{
    public interface IFight
    {
        FightDescription GetDescription();
        IList<CombatLogEvent> GetEvents();
        CombatLogEvent AddEvent(CombatLogEvent @event);
        (long Start, long End) Range { get; }
        void Parse();
        Task ParseAsync();
        bool IsEndEvent(IFightEnd type);
    }

    [DebuggerDisplay("{GetDescription()}")]
    public abstract class Fight<TStart, TEnd> : IFight
        where TStart : CombatLogEvent, IFightStart
        where TEnd : CombatLogEvent, IFightEnd
    {
        protected TStart _start;
        protected TEnd _end;
        protected List<CombatLogEvent> _events = new();

        public Fight(TStart start)
        {
            _start = start;
            _start.ParseAsync().Wait();
            _events.Add(start);
        }

        public CombatLogEvent AddEvent(CombatLogEvent @event)
        {
            _events.Add(@event);
            if (@event is TEnd endEvent)
            {
                _end = endEvent;
                _end.ParseAsync().Wait();
            }
            return @event;
        }        

        public void Sort() => _events = _events.OrderBy(x => x.Id).ToList();
        public IList<CombatLogEvent> GetEvents() => _events;
        public virtual FightDescription GetDescription() => new(Name, Duration, _start.Timestamp, Result);

        public void Parse()
        {
            _events.ForEach(e => e.Parse());
        }

        public async Task ParseAsync()
        {
            await Parallel.ForEachAsync(_events, async (e, _) => await e.ParseAsync());
        }

        public bool IsEndEvent(IFightEnd @event) => typeof(TEnd).IsAssignableFrom(@event.GetType());

        public TimeSpan Duration => _end is null ? (_events.Last().Timestamp - _start.Timestamp) : TimeSpan.FromMilliseconds(_end.Duration);
        public abstract string Name { get; }
        public abstract string Result { get; }
        public (long Start, long End) Range { get; set; }
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
        public override string ToString()
        {
            return $"{Description} ({Result}) {Duration} {Time}";
        }
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

    public class ArenaMatch : Fight<ArenaMatchStart, ArenaMatchEnd>
    {
        public ArenaMatch(ArenaMatchStart start) : base(start)
        {
        }

        public override string Name => _start.InstanceId.ToString();
        public override string Result => _end is ArenaMatchEnd endOfFight ? $"Team {endOfFight.WinningTeam} wins. New ratings: Team1 = {endOfFight.NewRatingTeam1}, Team2 = {endOfFight.NewRatingTeam2}" : "";
    }
}

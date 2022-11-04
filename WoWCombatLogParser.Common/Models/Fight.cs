using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace WoWCombatLogParser.Common.Models
{
    public partial interface IFight
    {
        FightDescription GetDescription();
        IList<CombatLogEvent> GetEvents();
        CombatLogEvent AddEvent(CombatLogEvent @event);
        (long Start, long End) Range { get; }
        void Parse();
        Task ParseAsync();
        bool IsEndEvent(IFightEnd type);
        FightDataDictionary CommonDataDictionary { get; }
    }

    [DebuggerDisplay("{GetDescription()}")]
    public abstract partial class Fight<TStart, TEnd> : IFight
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

        public abstract CombatLogEvent AddEvent(CombatLogEvent @event);

        public void Sort() => _events = _events.OrderBy(x => x.Id).ToList();
        
        public void Parse()
        {
            _events.ForEach(e => e.Parse());
        }

        public abstract Task ParseAsync();

        public IList<CombatLogEvent> GetEvents() => _events;
        public virtual FightDescription GetDescription() => new(Name, Duration, _start.Timestamp, Result);
        public bool IsEndEvent(IFightEnd @event) => typeof(TEnd).IsAssignableFrom(@event.GetType());
        public TimeSpan Duration => _end is null ? (_events.Last().Timestamp - _start.Timestamp) : TimeSpan.FromMilliseconds(_end.Duration);
        public abstract string Name { get; }
        public abstract string Result { get; }
        public (long Start, long End) Range { get; set; }
        public FightDataDictionary CommonDataDictionary { get; } = new();
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
}

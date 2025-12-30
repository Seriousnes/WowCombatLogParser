using System;
using System.Linq;

namespace WoWCombatLogParser
{

    /// <summary>
    /// Represents an abstract base class for a fight.
    /// </summary>
    /// <typeparam name="TStart">The type of the start event.</typeparam>
    /// <typeparam name="TEnd">The type of the end event.</typeparam>
    [DebuggerDisplay("{GetDetails()}")]
    public abstract partial class Fight<TStart, TEnd> : IFight
        where TStart : CombatLogEvent, IFightStart
        where TEnd : CombatLogEvent, IFightEnd
    {
        protected TStart _start;
        protected TEnd? _end;
        protected List<CombatLogEvent> _events = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Fight{TStart, TEnd}"/> class.
        /// </summary>
        /// <param name="start">The start event of the fight.</param>
        public Fight(TStart start)
        {
            _start = start;
            _events.Add(start);
        }

        /// <summary>
        /// Adds a combat log event to the fight.
        /// </summary>
        /// <param name="combatLogEvent">The combat log event to add.</param>
        /// <returns>The added <see cref="CombatLogEvent"/> object.</returns>
        public virtual CombatLogEvent AddEvent(CombatLogEvent combatLogEvent)
        {
            _events.Add(combatLogEvent);
            if (combatLogEvent is TEnd endEvent)
                _end = endEvent;
            return combatLogEvent;
        }

        /// <summary>
        /// Sorts the combat log events.
        /// </summary>
        public virtual void Sort() => _events = [.. _events.OrderBy(x => x.Timestamp).ThenBy(x => x.Id)];

        /// <summary>
        /// Gets the list of combat log events.
        /// </summary>
        /// <returns>A list of <see cref="CombatLogEvent"/> objects.</returns>
        public IList<CombatLogEvent> GetEvents() => _events;

        /// <summary>
        /// Gets the details of the fight.
        /// </summary>
        /// <returns>A <see cref="FightDescription"/> object containing the details of the fight.</returns>
        public virtual FightDescription GetDetails() => new(Name, Duration, _start.Timestamp, Result);

        /// <summary>
        /// Determines if the given event is an end event for the fight.
        /// </summary>
        /// <param name="combatLogEvent">The combat log event to check.</param>
        /// <returns>True if the event is an end event; otherwise, false.</returns>
        public virtual bool IsEndEvent(CombatLogEvent combatLogEvent) => combatLogEvent is TEnd;

        /// <summary>
        /// Gets the duration of the fight.
        /// </summary>
        public virtual TimeSpan Duration => _end is null ? (_events.Last().Timestamp - _start.Timestamp) : TimeSpan.FromMilliseconds(_end.Duration);

        /// <summary>
        /// Gets the name of the fight.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the result of the fight.
        /// </summary>
        public abstract string Result { get; }

        /// <summary>
        /// Gets the start and end range of the fight.
        /// </summary>
        public virtual (long Start, long End) Range { get; set; }

        /// <summary>
        /// Gets a value indicating whether the fight was successful.
        /// </summary>
        public abstract bool IsSuccess { get; }
    }
}

using System;
using System.Linq;

namespace WoWCombatLogParser
{
    /// <summary>
    /// Represents a trash fight.
    /// </summary>
    public class TrashEncounter : IFight
    {
        protected CombatLogEvent _start;
        protected CombatLogEvent? _end;
        protected List<CombatLogEvent> _events = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="TrashEncounter"/> class.
        /// </summary>
        /// <param name="start">The start event of the trash fight.</param>
        public TrashEncounter(CombatLogEvent start)
        {
            _events.Add(_start = start);
        }

        /// <summary>
        /// Sorts the combat log events.
        /// </summary>
        public void Sort() => _events = [.. _events.OrderBy(x => x.Timestamp).ThenBy(x => x.Id)];

        /// <summary>
        /// Gets the list of combat log events.
        /// </summary>
        /// <returns>A list of <see cref="CombatLogEvent"/> objects.</returns>
        public IList<CombatLogEvent> GetEvents() => _events;

        /// <summary>
        /// Gets the details of the trash fight.
        /// </summary>
        /// <returns>A <see cref="FightDescription"/> object containing the details of the fight.</returns>
        public virtual FightDescription GetDetails() => new(Name ?? "Unknown", Duration, _start.Timestamp, Result);

        /// <summary>
        /// Determines if the given event is an end event for the trash fight.
        /// </summary>
        /// <param name="combatLogEvent">The combat log event to check.</param>
        /// <returns>True if the event is an end event; otherwise, false.</returns>
        public bool IsEndEvent(CombatLogEvent combatLogEvent) => combatLogEvent is IFightEnd;

        /// <summary>
        /// Gets the duration of the trash fight.
        /// </summary>
        public TimeSpan Duration => _end is null ? (_events.Last().Timestamp - _start.Timestamp) : TimeSpan.Zero;

        /// <summary>
        /// Gets or sets the name of the trash fight.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets the result of the trash fight.
        /// </summary>
        public string Result { get; } = string.Empty;

        /// <summary>
        /// Gets the start and end range of the trash fight.
        /// </summary>
        public (long Start, long End) Range { get; set; }

        /// <summary>
        /// Gets a value indicating whether the trash fight was successful.
        /// </summary>
        public bool IsSuccess => true;

        /// <summary>
        /// Adds a combat log event to the trash fight.
        /// </summary>
        /// <param name="combatLogEvent">The combat log event to add.</param>
        /// <returns>The added <see cref="CombatLogEvent"/>.</returns>
        public CombatLogEvent AddEvent(CombatLogEvent combatLogEvent)
        {
            _events.Add(combatLogEvent);
            return combatLogEvent;
        }
    }
}

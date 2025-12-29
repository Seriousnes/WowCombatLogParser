using System;
using System.Linq;

namespace WoWCombatLogParser
{
    /// <summary>
    /// Represents a collection of <see cref="CombatLogEvent"/> objects relating to a specific encounter.
    /// </summary>
    public interface IFight
    {
        /// <summary>
        /// Sorts the combat log events.
        /// </summary>
        void Sort();

        /// <summary>
        /// Gets the details of the fight.
        /// </summary>
        /// <returns>A <see cref="FightDescription"/> object containing the details of the fight.</returns>
        FightDescription GetDetails();

        /// <summary>
        /// Gets the list of combat log events.
        /// </summary>
        /// <returns>A list of <see cref="CombatLogEvent"/> objects.</returns>
        IList<CombatLogEvent> GetEvents();

        /// <summary>
        /// Adds a combat log event to the fight.
        /// </summary>
        /// <param name="combatLogEvent">The combat log event to add.</param>
        /// <returns>The added <see cref="CombatLogEvent"/>.</returns>
        CombatLogEvent AddEvent(CombatLogEvent combatLogEvent);

        /// <summary>
        /// Gets the start and end range of the fight.
        /// </summary>
        (long Start, long End) Range { get; }

        /// <summary>
        /// Determines if the given event is an end event for the fight.
        /// </summary>
        /// <param name="combatLogEvent">The combat log event to check.</param>
        /// <returns>True if the event is an end event; otherwise, false.</returns>
        bool IsEndEvent(CombatLogEvent combatLogEvent);

        /// <summary>
        /// Gets the name of the fight.
        /// </summary>
        string? Name { get; }

        /// <summary>
        /// Gets a value indicating whether the fight was successful.
        /// </summary>
        bool IsSuccess { get; }
    }

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
        public virtual void Sort() => _events = [.. _events.OrderBy(x => x.Id)];

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

    /// <summary>
    /// Describes the details of a fight.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="FightDescription"/> class.
    /// </remarks>
    /// <param name="description">Typically the boss's name, arena map, or name of the dungeon/challenge mode.</param>
    /// <param name="duration">Duration of the fight.</param>
    /// <param name="time">Timestamp of the initial combat log event.</param>
    /// <param name="result">Typically either Kill/Wipe in boss-type encounters, Timed/Not-timed for challenge modes, or win/loss for PvP.</param>
    [DebuggerDisplay("{Description} ({Result}) {Duration} {Time}")]
    public class FightDescription(string description, TimeSpan duration, DateTime time, string result)
    {

        /// <summary>
        /// Gets the description of the fight.
        /// </summary>
        public string Description { get; init; } = description;

        /// <summary>
        /// Gets the duration of the fight.
        /// </summary>
        public string Duration { get; init; } = $"{duration:m\\:ss}";

        /// <summary>
        /// Gets the result of the fight.
        /// </summary>
        public string Result { get; init; } = result;

        /// <summary>
        /// Gets the start time of the fight.
        /// </summary>
        public DateTime Time { get; init; } = time;

        /// <returns>A string representation of the fight description.</returns>
        public override string ToString()
        {
            return $"{Description} ({Result}) {Duration} {Time:h:mm tt}";
        }
    }

    /// <summary>
    /// Represents a boss fight.
    /// </summary>
    /// <param name="start">The start event of the boss fight.</param>
    public class Boss(EncounterStart start) : Fight<EncounterStart, EncounterEnd>(start)
    {
        /// <summary>
        /// Adds a combat log event to the boss fight.
        /// </summary>
        /// <param name="combatLogEvent">The combat log event to add.</param>
        /// <returns>The added <see cref="CombatLogEvent"/>.</returns>
        public override CombatLogEvent AddEvent(CombatLogEvent combatLogEvent)
        {
            base.AddEvent(combatLogEvent);

            if (combatLogEvent is ICombatantInfo combatantInfoEvent)
            {
                Combatants.Add(combatantInfoEvent);
            }

            return combatLogEvent;
        }

        /// <summary>
        /// Gets the name of the boss.
        /// </summary>
        public override string Name => _start.Name ?? "Unknown";

        /// <summary>
        /// Gets the result of the boss fight.
        /// </summary>
        public override string Result => _end is EncounterEnd endOfFight && endOfFight.Success ? "Kill" : "Wipe";

        /// <summary>
        /// Gets a value indicating whether the boss fight was successful.
        /// </summary>
        public override bool IsSuccess => Result == "Kill";

        /// <summary>
        /// Gets the list of combatants in the boss fight.
        /// </summary>
        public virtual List<ICombatantInfo> Combatants { get; } = new();
    }

    /// <summary>
    /// Represents a trash fight.
    /// </summary>
    public class Trash : IFight
    {
        protected CombatLogEvent _start;
        protected CombatLogEvent? _end;
        protected List<CombatLogEvent> _events = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Trash"/> class.
        /// </summary>
        /// <param name="start">The start event of the trash fight.</param>
        public Trash(CombatLogEvent start)
        {
            _events.Add(_start = start);
        }

        /// <summary>
        /// Sorts the combat log events.
        /// </summary>
        public void Sort() => _events = [.. _events.OrderBy(x => x.Id)];

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

    /// <summary>
    /// Represents a challenge mode fight.
    /// </summary>
    /// <param name="start">The start event of the challenge mode fight.</param>
    public class ChallengeMode(ChallengeModeStart start) : Fight<ChallengeModeStart, ChallengeModeEnd>(start)
    {

        /// <summary>
        /// Gets the name of the challenge mode.
        /// </summary>
        public override string Name => $"{_start.InstanceId} Level {_start.KeystoneLevel} (Affixes: {string.Join(',', _start.Affixes?.Select(x => x.Id.ToString()) ?? [])})";

        /// <summary>
        /// Gets the result of the challenge mode.
        /// </summary>
        public override string Result => _end is ChallengeModeEnd endOfFight && endOfFight.Success ? "Timed" : "Not timed";

        /// <summary>
        /// Gets a value indicating whether the challenge mode was successful.
        /// </summary>
        public override bool IsSuccess => Result == "Timed";
    }

    /// <summary>
    /// Represents an arena match.
    /// </summary>
    /// <param name="start">The start event of the arena match.</param>
    public class ArenaMatch(ArenaMatchStart start) : Fight<ArenaMatchStart, ArenaMatchEnd>(start)
    {

        /// <summary>
        /// Gets the name of the arena match.
        /// </summary>
        public override string Name => _start.InstanceId.ToString();

        /// <summary>
        /// Gets the result of the arena match.
        /// </summary>
        public override string Result => _end is ArenaMatchEnd endOfFight ? $"Team {endOfFight.WinningTeam} wins. New ratings: Team1 = {endOfFight.NewRatingTeam1}, Team2 = {endOfFight.NewRatingTeam2}" : "";

        /// <summary>
        /// Gets a value indicating whether the arena match was successful.
        /// </summary>
        public override bool IsSuccess => Result.Contains("wins", StringComparison.InvariantCultureIgnoreCase);
    }
}

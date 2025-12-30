using System;

namespace WoWCombatLogParser
{
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
}

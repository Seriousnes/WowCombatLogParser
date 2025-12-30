using System;

namespace WoWCombatLogParser
{
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

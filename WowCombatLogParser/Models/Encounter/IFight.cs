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
        (long Start, long End) Range { get; set; }

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
}

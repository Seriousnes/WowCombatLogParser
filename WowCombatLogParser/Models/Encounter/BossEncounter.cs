namespace WoWCombatLogParser
{
    /// <summary>
    /// Represents a boss fight.
    /// </summary>
    /// <param name="start">The start event of the boss fight.</param>
    public class BossEncounter(EncounterStart start) : Fight<EncounterStart, EncounterEnd>(start)
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
}

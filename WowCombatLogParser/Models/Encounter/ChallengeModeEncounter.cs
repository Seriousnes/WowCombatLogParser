using System;
using System.Linq;

namespace WoWCombatLogParser
{
    /// <summary>
    /// Represents a challenge mode fight.
    /// </summary>
    /// <param name="start">The start event of the challenge mode fight.</param>
    public class ChallengeModeEncounter(ChallengeModeStart start) : Fight<ChallengeModeStart, ChallengeModeEnd>(start)
    {

        private BossEncounter? currentBoss;
        private TrashEncounter? currentTrash;

        public List<IFight> Encounters { get; } = new();

        public List<BossEncounter> Bosses { get; } = new();

        public List<TrashEncounter> Trash { get; } = new();

        /// <summary>
        /// Gets the name of the challenge mode.
        /// </summary>
        public override string Name => $"{_start.ZoneName} +{_start.KeystoneLevel} (Affixes: {string.Join(',', _start.Affixes?.Select(x => x.Id.GetDescription()) ?? [])})";

        /// <summary>
        /// Gets the result of the challenge mode.
        /// </summary>
        public override string Result => _end is ChallengeModeEnd endOfFight && endOfFight.Success ? "Timed" : "Not timed";

        /// <summary>
        /// Gets a value indicating whether the challenge mode was successful.
        /// </summary>
        public override bool IsSuccess => Result is "Timed";

        public override CombatLogEvent AddEvent(CombatLogEvent combatLogEvent)
        {
            base.AddEvent(combatLogEvent);

            switch (combatLogEvent)
            {
                case EncounterStart startEvent:
                    FinalizeTrashIfPresent();
                    currentBoss = new BossEncounter(startEvent);
                    return combatLogEvent;

                case EncounterEnd endEvent when currentBoss is not null:
                    currentBoss.AddEvent(endEvent);
                    FinalizeBoss();
                    return combatLogEvent;

                case ICombatantInfo:
                    currentBoss?.AddEvent(combatLogEvent);
                    return combatLogEvent;

                case ChallengeModeEnd:
                    FinalizeBossIfPresent();
                    FinalizeTrashIfPresent();
                    return combatLogEvent;

                default:
                    if (currentBoss is not null)
                    {
                        currentBoss.AddEvent(combatLogEvent);
                        return combatLogEvent;
                    }

                    if (currentTrash is null)
                    {
                        currentTrash = new TrashEncounter(combatLogEvent);
                        return combatLogEvent;
                    }

                    currentTrash.AddEvent(combatLogEvent);
                    return combatLogEvent;
            }
        }

        private void FinalizeBossIfPresent()
        {
            if (currentBoss is null)
            {
                return;
            }

            FinalizeBoss();
        }

        private void FinalizeBoss()
        {
            if (currentBoss is null)
            {
                throw new InvalidOperationException("No boss encounter is active.");
            }

            currentBoss.Sort();
            Encounters.Add(currentBoss);
            Bosses.Add(currentBoss);
            currentBoss = null;
        }

        private void FinalizeTrashIfPresent()
        {
            if (currentTrash is null)
            {
                return;
            }

            currentTrash.Sort();
            Encounters.Add(currentTrash);
            Trash.Add(currentTrash);
            currentTrash = null;
        }
    }
}

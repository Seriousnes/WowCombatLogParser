namespace WoWCombatLogParser.Events
{
    [DebuggerDisplay("{Spell}")]
    public abstract class AbilityBase : EventSection
    {
        public Ability Spell { get; } = new();
    }
}

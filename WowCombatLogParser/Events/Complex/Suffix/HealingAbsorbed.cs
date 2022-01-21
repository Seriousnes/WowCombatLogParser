namespace WoWCombatLogParser.Events
{
    [Suffix("_HEAL_ABSORBED")]
    [DebuggerDisplay("{ExtraUnit} {ExtraSpell} {ExtraAmount}")]
    public class HealingAbsorbed : EventSection
    {
        public Unit ExtraUnit { get; set; } = new();
        public Ability ExtraSpell { get; set; } = new();
        public int ExtraAmount { get; set; }
    }
}

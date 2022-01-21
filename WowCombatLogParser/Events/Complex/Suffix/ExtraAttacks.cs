namespace WoWCombatLogParser.Events
{
    [Suffix("_EXTRA_ATTACKS")]
    public class ExtraAttacks : EventSection
    {
        public int Amount { get; set; }
    }
}

namespace WoWCombatLogParser.Events
{
    [Suffix("_AURA_APPLIED")]
    [DebuggerDisplay("{AuraType} {Amount}")]
    public class AuraApplied : EventSection
    {
        public AuraType AuraType { get; set; }
        public decimal Amount { get; set; }
    }
}

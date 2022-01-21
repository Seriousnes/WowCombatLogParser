namespace WoWCombatLogParser.Events
{
    [Suffix("_AURA_REMOVED")]
    [DebuggerDisplay("{AuraType} {Amount}")]
    public class AuraRemoved : EventSection
    {
        public AuraType AuraType { get; set; }
        public decimal Amount { get; set; }
    }
}

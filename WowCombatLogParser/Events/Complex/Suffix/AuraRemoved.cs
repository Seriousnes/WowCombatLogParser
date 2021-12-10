using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex
{
    [Suffix("_AURA_REMOVED")]
    [DebuggerDisplay("{AuraType} {Amount}")]
    public class AuraRemoved : IEventSection
    {
        public AuraType AuraType { get; set; }
        public decimal Amount { get; set; }
    }
}

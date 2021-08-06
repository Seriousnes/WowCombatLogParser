using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex
{
    [Suffix("_AURA_APPLIED")]
    public class AuraApplied : IEventSection
    {
        public AuraType AuraType { get; set; }
        public decimal Amount { get; set; }
    }
}

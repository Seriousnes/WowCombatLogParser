using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex
{
    [Suffix("_AURA_REMOVED")]
    public class AuraRemoved : IEventSection
    {
        public string AuraType { get; set; }
        public decimal Amount { get; set; }
    }
}

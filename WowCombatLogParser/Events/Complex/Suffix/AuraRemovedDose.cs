using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex
{
    [Suffix("_AURA_REMOVED_DOSE")]
    public class AuraRemovedDose : IEventSection
    {
        public string AuraType { get; set; }
        public decimal Stacks { get; set; }
    }
}

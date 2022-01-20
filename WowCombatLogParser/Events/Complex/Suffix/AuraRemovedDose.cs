using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex
{
    [Suffix("_AURA_REMOVED_DOSE")]
    [DebuggerDisplay("{AuraType} {Stacks}")]
    public class AuraRemovedDose : EventSection
    {
        public AuraType AuraType { get; set; }
        public decimal Stacks { get; set; }
    }
}

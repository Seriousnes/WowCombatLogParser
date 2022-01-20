using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex
{
    [Suffix("_AURA_APPLIED_DOSE")]
    [DebuggerDisplay("{AuraType} {Stacks}")]
    public class AuraAppliedDose : EventSection
    {
        public AuraType AuraType { get; set; }
        public decimal Stacks { get; set; }
    }
}

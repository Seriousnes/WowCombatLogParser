using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    [Suffix("_AURA_APPLIED_DOSE")]
    [DebuggerDisplay("{AuraType} {Stacks}")]
    public class AuraAppliedDose : EventSection, IAura
    {
        public AuraType AuraType { get; set; }
        public decimal Stacks { get; set; }
    }
}

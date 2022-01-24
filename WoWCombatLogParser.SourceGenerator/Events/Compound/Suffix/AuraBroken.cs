using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    [Suffix("_AURA_BROKEN")]
    public class AuraBroken : EventSection
    {
        public AuraType Type { get; set; }
    }

    [Suffix("_AURA_BROKEN_SPELL")]
    public class AuraBrokenSpell : SuffixAbilityBase
    {
        public AuraType AuraType { get; set; }
    }

}

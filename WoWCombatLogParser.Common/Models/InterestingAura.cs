using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    public class InterestingAura : EventSection
    {
        public WowGuid PlayerId { get; set; }
        public int AuraId { get; set; }
    }
}

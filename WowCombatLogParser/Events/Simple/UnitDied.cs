using WoWCombatLogParser.Events.Parts;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Simple
{
    [Affix("UNIT_DIED")]
    public class UnitDied : IEventSection
    {
        public WowGuid RecapId { get; set; } = new();
        public bool UnconsciousOnDeath { get; set; }
        // skip next two fields
        [Offset(2)]
        public Unit Unit { get; set; } = new();
    }
}

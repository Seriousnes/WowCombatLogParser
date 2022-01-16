using WoWCombatLogParser.Events.Parts;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Special
{
    [Affix("UNIT_DIED")]
    public class UnitDied : Part
    {
        public WowGuid RecapId { get; set; }
        public bool UnconsciousOnDeath { get; set; }
        public object UnusedField1 { get; set; }
        public object UnusedField2 { get; set; }
        public Unit Unit { get; set; } = new();
    }
}

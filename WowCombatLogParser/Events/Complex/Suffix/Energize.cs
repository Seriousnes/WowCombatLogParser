using WoWCombatLogParser.Events.Parts;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex
{
    [Suffix("_ENERGIZE")]
    [DebuggerDisplay("{OverEnergize} {PowerType} {AlternatePowerType}")]
    public class Energize : AdvancedLoggingDetailsBase<int>, IEventSection
    {
        public int OverEnergize { get; set; }
        public PowerType PowerType { get; set; }
        public PowerType AlternatePowerType { get; set; }
    }
}

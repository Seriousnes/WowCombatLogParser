using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    [Suffix("_DRAIN")]
    [DebuggerDisplay("{OverEnergize} {PowerType} {AlternatePowerType}")]
    public class Drain : AdvancedLoggingDetailsBase, IDrain 
    {
        public PowerType PowerType { get; set; }
        public decimal ExtraAmount { get; set; }
        public decimal MaxPower { get; set; }
    }

    [Suffix("_LEECH")]
    [DebuggerDisplay("{OverEnergize} {PowerType} {AlternatePowerType}")]
    public class Leech : AdvancedLoggingDetailsBase, IDrain
    {
        public PowerType PowerType { get; set; }
        public decimal ExtraAmount { get; set; }
    }
}

namespace WoWCombatLogParser.Events
{
    [Suffix("_ENERGIZE")]
    [DebuggerDisplay("{OverEnergize} {PowerType} {AlternatePowerType}")]
    public class Energize : AdvancedLoggingDetailsBase<decimal>
    {
        public decimal OverEnergize { get; set; }
        public PowerType PowerType { get; set; }
        public PowerType AlternatePowerType { get; set; }
    }
}

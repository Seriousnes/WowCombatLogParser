using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Parts
{
    [DebuggerDisplay("{InfoGuid} {OwnerGuid} {UnitInfo} {Location}")]
    public class AdvancedLoggingDetails : EventSection
    {
        public WowGuid InfoGuid { get; set; }
        public WowGuid OwnerGuid { get; set; }
        public UnitInfo UnitInfo { get; set; } = new();
        public Location Location { get; set; } = new();
        
    }

    [DebuggerDisplay("{AdvancedLoggingDetails} {Amount}")]
    public class AdvancedLoggingDetailsBase<T>: EventSection
    {
        public AdvancedLoggingDetails AdvancedLoggingDetails { get; set; } = new();
        public T Amount { get; set; }
    }

    [DebuggerDisplay("{CurrentHP} {MaxHP} {AttackPower} {SpellPower} {Armor} {Absorb} {Power}")]
    public class UnitInfo : EventSection
    {
        public int CurrentHP { get; set; }
        public int MaxHP { get; set; }
        public int AttackPower { get; set; }
        public int SpellPower { get; set; }
        public int Armor { get; set; }
        public int Absorb { get; set; }
        public Power Power { get; set; } = new();
    }

    [DebuggerDisplay("{PowerType} {CurrentPower} {MaxPower} {PowerCost}")]
    public class Power : EventSection
    {
        public PowerType PowerType { get; set; }
        public decimal CurrentPower { get; set; }
        public decimal MaxPower { get; set; }
        public decimal PowerCost { get; set; }
    }

    [DebuggerDisplay("{X} {Y} {MapId} {Facing} {Level}")]
    public class Location : EventSection
    {
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public int MapId { get; set; }
        public decimal Facing { get; set; }
        public decimal Level { get; set; }
    }
}

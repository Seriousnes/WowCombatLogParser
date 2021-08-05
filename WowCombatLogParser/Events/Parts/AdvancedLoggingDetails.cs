using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Parts
{
    public class AdvancedLoggingDetails : IEventSection
    {
        public string InfoGuid { get; set; }
        public string OwnerGuid { get; set; }
        public UnitInfo UnitInfo { get; set; } = new();
        public Location Location { get; set; } = new();
    }

    public class UnitInfo : IEventSection
    {
        public int CurrentHP { get; set; }
        public int MaxHP { get; set; }
        public int AttackPower { get; set; }
        public int SpellPower { get; set; }
        public int Armor { get; set; }
        public Power Power { get; set; } = new();
    }

    public class Power : IEventSection
    {
        [Offset(1)]
        public PowerType PowerType { get; set; }
        public decimal CurrentPower { get; set; }
        public decimal MaxPower { get; set; }
        public decimal PowerCost { get; set; }
    }

    public class Location : IEventSection
    {
        public decimal X { get; set; }
        public decimal Y { get; set; }
        public int MapId { get; set; }
        public decimal Facing { get; set; }
        public decimal Level { get; set; }
    }
}

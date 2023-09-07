using System.Diagnostics;
using WoWCombatLogParser.Common.Events;

namespace WoWCombatLogParser.Common.Models;

[DebuggerDisplay("{InfoGuid} {OwnerGuid} {UnitInfo} {Location} {Level}")]
public class AdvancedLoggingDetails : Event, IAdvancedDetails
{
    public WowGuid InfoGuid { get; set; }
    public WowGuid OwnerGuid { get; set; }
    public UnitInfo UnitInfo { get; } = new UnitInfo();
    public Location Location { get; } = new Location();
    public int Level { get; set; }
}

[DebuggerDisplay("{AdvancedLoggingDetails} {Amount}")]
public class AdvancedLoggingDetailsBase : Event
{
    public AdvancedLoggingDetails AdvancedLoggingDetails { get; } = new AdvancedLoggingDetails();
    public decimal Amount { get; set; }
}

[DebuggerDisplay("{CurrentHP} {MaxHP} {AttackPower} {SpellPower} {Armor} {Absorb} {Power}")]
public class UnitInfo : Event
{
    public int CurrentHP { get; set; }
    public int MaxHP { get; set; }
    public int AttackPower { get; set; }
    public int SpellPower { get; set; }
    public int Armor { get; set; }
    public int Absorb { get; set; }
    public Power Power { get; } = new Power();
}

[DebuggerDisplay("{PowerType} {CurrentPower} {MaxPower} {PowerCost}")]
public class Power : Event
{
    public PowerType PowerType { get; set; }
    public decimal CurrentPower { get; set; }
    public decimal MaxPower { get; set; }
    public decimal PowerCost { get; set; }
}

[DebuggerDisplay("{X} {Y} {MapId} {Facing}")]
public class Location : Event
{
    public decimal X { get; set; }
    public decimal Y { get; set; }
    public int MapId { get; set; }
    public decimal Facing { get; set; }
}

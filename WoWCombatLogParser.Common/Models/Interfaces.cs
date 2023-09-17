using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Events;

namespace WoWCombatLogParser.Common.Models;

public interface ICombatLogEvent : ICombatLogEventComponent
{
    int Id { get; }
    DateTime Timestamp { get; set; }
    string Name { get; }
}

public interface ICombatantInfo
{
    public WowGuid PlayerGuid { get; set; }
    public Faction Faction { get; set; }
}

/// <summary>
/// CombatLogEvents with a source and destination
/// </summary>
public interface IAction
{
    Unit Source { get; set; }
    Unit Destination { get; set; }
}

public interface IAbility
{
    Ability Spell { get; set; }
}

public interface IDamageOrHealing
{
    public decimal Amount { get; }
}

public interface IHealing : IDamageOrHealing
{
    int Overhealing { get; }
    int Absorbed { get; }
    bool Critical { get; }
}

public interface IDamage : IDamageOrHealing
{
    decimal UnmitigatedAmount { get; }
    SpellSchool School { get; }
    decimal Resisted { get; }
    decimal Blocked { get; }
    decimal Absorbed { get; }
    bool Critical { get; }
    bool Crushing { get; }
    bool IsOffHand { get; }
}

public interface IFightStart
{
}

public interface IFightEnd
{
    int Duration { get; }
}

public interface IFightEndSuccess : IFightEnd
{
    bool Success { get; }
}

public interface IAura
{
    AuraType AuraType { get; }
}

public interface IDrain
{
    PowerType PowerType { get; set; }
    decimal ExtraAmount { get; set; }
}

public interface IAdvancedDetails
{
    WowGuid InfoGuid { get; }
    WowGuid OwnerGuid { get; }
    UnitInfo UnitInfo { get; }
    Location Location { get; }
    int Level { get; }
}

public interface IAdvancedLoggingDetails
{
    AdvancedLoggingDetails AdvancedLoggingDetails { get; }
}

public interface ICast
{
}

public interface IKey
{
    bool EqualsKey(IKey key);
}

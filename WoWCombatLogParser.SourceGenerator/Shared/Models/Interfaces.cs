﻿using System;

namespace WoWCombatLogParser;

public interface ICombatLogEvent : ICombatLogEventComponent
{
    int Id { get; }
    DateTime Timestamp { get; set; }
    string EventName { get; }
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
    Actor Source { get; set; }
    Actor Destination { get; set; }
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

public interface IEmpowerFinish
{
    int Stage { get; }
}

public interface IHasSource
{
    Actor Source { get; }
}

public interface IHasDestination
{
    Actor Destination { get; }
}

﻿using System;
using System.ComponentModel;

namespace WoWCombatLogParser;

public enum FieldIndex
{
    Timestamp = 0,
    EventType = 1
}

public enum EncounterType
{
    DungeonOrRaid,
    ChallengeMode,
    PvP
}

public enum Reaction
{
    Neutral,
    Friendly,
    Hostile
}

public enum InstanceType
{
    Party,
    Raid,
    Scenario,
    PvP,
    None
}

public enum Difficulty
{
    Normal = 1,
    Heroic,
    Player10,
    Player25,
    Player10Heroic,
    Player25Heroic,
    LookingForRaid_Legacy,
    MythicKeystone,
    Player40,
    HeroicScenario = 11,
    NormalScenario,
    NormalRaid = 14,
    HeroicRaid,
    MythicRaid,
    LookingForRaid,
    Event_Raid,
    Event_Party,
    EventScenario_Scenario,
    MythicDungeon = 23,
    Timewalking,
    WorldPvPScenario,
    PvEvPScenario = 29,
    EventScenario,
    WorldPvPScenario1 = 32,
    TimewalkingRaid,
    PvP,
    Normal_Scenario = 38,
    Heroic_Scenario,
    Mythic_Scenario,
    PvP_Scenario = 45,
    Normal_Scenario_Warfronts = 147,
    Heroic_Scenario_Warfronts = 149,
    Normal_Party,
    LFR_Timewalking,
    VisionsOfNZoth,
    TeemingIsland,
    Torghast = 167,
    PathOfAscension_Courage,
    PathOfAscension_Loyalty,
    PathOfAscension_Wisdom,
    PathOfAscension_Humility,
    WorldBoss
}

public class DifficultyInfo
{
    public string Name { get; set; }
    public InstanceType Type { get; set; }
}

public enum UnitType
{
    Player,
    Pet,
    NPC,
}

public enum EnvironmentalType
{
    Drowning,
    Falling,
    Fatigue,
    Fire,
    Lava,
    Slime
}

public enum AuraType
{
    BUFF,
    DEBUFF
}

public enum MissType
{
    ABSORB,
    BLOCK,
    DEFLECT,
    DODGE,
    EVADE,
    IMMUNE,
    MISS,
    PARRY,
    REFLECT,
    RESIST
}

public enum Faction
{
    Horde = 0,
    Alliance
}

public enum SpellSchool
{
    None = 0,
    Physical = 0x1,
    Holy = 0x2,
    [Description("Holystrike")]
    Celestial = Holy | Physical,
    Fire = 0x4,
    [Description("Flamestrike")]
    Inferno = Fire | Physical,
    Radiant = Holy | Fire,
    Nature = 0x8,
    [Description("Stormstrike")]
    Tempest = Nature | Physical,
    Holystorm = Nature | Holy,
    Volcanic = Nature | Fire,
    Frost = 0x10,
    [Description("Froststrike")]
    Arctic = Frost | Physical,
    Holyfrost = Frost | Holy,
    Frostfire = Frost | Fire,
    Froststorm = Frost | Nature,
    Elemental = Frost | Nature | Fire,
    Shadow = 0x20,
    [Description("Shadowstrike")]
    Nightfall = Shadow | Physical,
    Twilight = Shadow | Holy,
    Shadowflame = Shadow | Fire,
    Plague = Shadow | Nature,
    Shadowfrost = Shadow | Frost,
    Arcane = 0x40,
    [Description("Spellstrike")]
    Mystic = Arcane | Physical,
    Divine = Arcane | Holy,
    Spellfire = Arcane | Fire,
    Astral = Arcane | Nature,
    Spellfrost = Arcane | Frost,
    Spellshadow = Arcane | Shadow,
    Chromatic = Spellshadow | Elemental,
    Magic = Chromatic | Holy,
    Chaos = Magic | Physical,
    Cosmic = Divine | Plague
}

public enum PowerType
{
    HealthCost = -2,
    None,
    Mana,
    Rage,
    Focus,
    Energy,
    ComboPoints,
    Runes,
    RunicPower,
    SoulShards,
    LunaPower,
    HolyPower,
    Alternate,
    Chi,
    Insanity,
    Obsolete,
    Obsolete2,
    ArcaneCharges,
    Fury,
    Pain,
    NumPowerTypes
}

[Flags]
public enum AffiliationFlag : uint
{
    Undefined = 0,
    Mine = 0x00000001,
    Party = 0x00000002,
    Raid = 0x00000004,
    Outsider = 0x00000008,
    Mask = 0x0000000f,
}

[Flags]
public enum ReactionFlag : uint
{
    Undefined = 0,
    Friendly = 0x00000010,
    Neutral = 0x00000020,
    Hostile = 0x00000040,
    Mask = 0x000000f0,
}

[Flags]
public enum OwnershipFlag : uint
{
    Undefined = 0,
    Player = 0x00000100,
    Npc = 0x00000200,
    Mask = 0x00000300,
}

[Flags]
public enum UnitTypeFlag : uint
{
    Undefined = 0,
    Player = 0x00000400,
    Npc = 0x00000800,
    Pet = 0x00001000,
    Guardian = 0x00002000,
    Object = 0x00004000,
    Mask = 0x0000fc00,
}

[Flags]
public enum SpecialFlag : uint
{
    Undefined = 0,
    Target = 0x00010000,
    Focus = 0x00020000,
    Maintank = 0x00040000,
    Mainassist = 0x00080000,
    None = 0x80000000,
    Mask = 0xffff0000,
}

[Flags]
public enum RaidFlag : uint
{
    Star = 0x1,
    Circle = 0x2,
    Diamond = 0x4,
    Triangle = 0x8,
    Moon = 0x10,
    Square = 0x20,
    Cross = 0x40,
    Skull = 0x80,
    RaidTargetMask = 0xff
}

public enum WorldMarker
{
    Square = 1,
    Triangle,
    Diamond,
    Cross,
    Star,
    Circle,
    Moon,
    Skull
}

public enum Soulbind
{
    Niya = 1,
    Dreamweaver,
    GeneralDraven,
    PlagueDeviserMarileth,
    Emeni,
    Korayn,
    Pelagos,
    NadjiaTheMistblade,
    TheotarTheMadDuke,
    BonesmithHeirmir,
    Kleia,
    ForgelitePrimeMikanikos
}

public enum Covenant
{
    Kyrian = 1,
    Venthyr,
    NightFae,
    Necrolord
}

public enum MatchType
{
    Skirmish,
    Rated,
    [Description("Rated Solo Shuffle")]
    RatedSoloShuffle
}

public enum CombatLogVersion
{
    Any = -1,
    Wotlk = 9,
    Shadowlands = 19,
    Dragonflight = 20
}

public enum ChallengeModeAffixEnum
{
    Placeholder = 133,
    [KeystoneLevel(Level = 1)]
    Fortified = 10,
    [KeystoneLevel(Level = 1)]
    Tyrannical = 9,
    [KeystoneLevel(Level = 4)]
    Bolstering = 7,
    [KeystoneLevel(Level = 4)]
    Bursting = 11,
    [KeystoneLevel(Level = 4)]
    Inspiring = 122,
    [KeystoneLevel(Level = 4)]
    Raging = 6,
    [KeystoneLevel(Level = 4)]
    Sanguine = 8,
    [KeystoneLevel(Level = 4)]
    Spiteful = 123,
    [KeystoneLevel(Level = 4)]
    Teeming = 5,
    [KeystoneLevel(Level = 7)]
    Explosive = 13,
    [KeystoneLevel(Level = 7)]
    Grievous = 12,
    [KeystoneLevel(Level = 7)]
    Necrotic = 4,
    [KeystoneLevel(Level = 7)]
    Overflowing = 1,
    [KeystoneLevel(Level = 7)]
    Quaking = 14,
    [KeystoneLevel(Level = 7)]
    Skittish = 2,
    [KeystoneLevel(Level = 7)]
    Storming = 124,
    [KeystoneLevel(Level = 7)]
    Volcanic = 3,
    [KeystoneLevel(Level = 10)]
    Awakened = 120,
    [KeystoneLevel(Level = 10)]
    Beguiling = 119,
    [KeystoneLevel(Level = 10)]
    Encrypted = 130,
    [KeystoneLevel(Level = 10)]
    Infernal = 129,
    [KeystoneLevel(Level = 10)]
    Infested = 16,
    [KeystoneLevel(Level = 10)]
    Prideful = 121,
    [KeystoneLevel(Level = 10)]
    Reaping = 117,
    [KeystoneLevel(Level = 10)]
    Shrouded = 131,
    [KeystoneLevel(Level = 10)]
    Thundering = 132,
    [KeystoneLevel(Level = 10)]
    Tormented = 128
}

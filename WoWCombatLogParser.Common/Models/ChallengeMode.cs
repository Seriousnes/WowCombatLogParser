using System;

namespace WoWCombatLogParser.Common.Models;

[AttributeUsage(AttributeTargets.Field)]
public class KeystoneLevelAttribute : Attribute
{
    public KeystoneLevelAttribute()         
    {
    }

    public int Level { get; set; }
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

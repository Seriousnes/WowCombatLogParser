using System;

namespace WoWCombatLogParser;

[AttributeUsage(AttributeTargets.Field)]
public class KeystoneLevelAttribute : Attribute
{
    public KeystoneLevelAttribute()
    {
    }

    public int Level { get; set; }
}
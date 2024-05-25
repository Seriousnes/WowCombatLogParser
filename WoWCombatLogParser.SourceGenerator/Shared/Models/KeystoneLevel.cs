using System;

namespace WoWCombatLogParser.Models;

[AttributeUsage(AttributeTargets.Field)]
public class KeystoneLevelAttribute : Attribute
{
    public KeystoneLevelAttribute()
    {
    }

    public int Level { get; set; }
}
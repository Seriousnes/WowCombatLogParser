using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWCombatLogParser.Models;

[AttributeUsage(AttributeTargets.Field)]
internal class KeystoneLevelAttribute : Attribute
{
    public KeystoneLevelAttribute()
    {
    }

    public int Level { get; set; }
}
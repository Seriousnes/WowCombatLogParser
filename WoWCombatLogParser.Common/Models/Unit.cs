﻿using System.Diagnostics;
using WoWCombatLogParser.Common.Events;

namespace WoWCombatLogParser.Common.Models
{
    [DebuggerDisplay("{Id} {Name} {Flags} {RaidFlags}")]
    public class Unit : EventSection
    {
        public WowGuid Id { get; set; }
        public string Name { get; set; }
        public UnitFlag Flags { get; set; }
        public RaidFlag RaidFlags { get; set; }
    }
}
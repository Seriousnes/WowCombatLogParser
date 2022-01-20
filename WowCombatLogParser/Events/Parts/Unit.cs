using System.Diagnostics;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Parts
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

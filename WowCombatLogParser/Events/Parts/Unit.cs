using System.Diagnostics;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Parts
{
    [DebuggerDisplay("{Name}")]
    public class Unit : IEventSection
    {
        public WowGuid Id { get; set; } = new();
        public string Name { get; set; }
        public long Flags { get; set; }
        public long RaidFlags { get; set; }
    }
}

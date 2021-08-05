using System.Diagnostics;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Parts
{
    [DebuggerDisplay("{Name}")]
    public class Actor : IEventSection
    {
        public WowGuid Id { get; set; }
        public string Name { get; set; }
        public long Flags { get; set; }
        public long RaidFlags { get; set; }
    }
}

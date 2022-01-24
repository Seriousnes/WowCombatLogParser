using System.Diagnostics;
using WoWCombatLogParser.Common.Events;

namespace WoWCombatLogParser.Common.Models
{
    [DebuggerDisplay("{Id} {Name} {School}")]
    public class Ability : EventSection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SpellSchool School { get; set; }
    }
}

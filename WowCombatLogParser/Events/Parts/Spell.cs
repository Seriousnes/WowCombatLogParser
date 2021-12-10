using System.Diagnostics;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Parts
{
    [DebuggerDisplay("{Id} {Name} {School}")]
    public class Spell : IEventSection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SpellSchool School { get; set; }        
    }
}

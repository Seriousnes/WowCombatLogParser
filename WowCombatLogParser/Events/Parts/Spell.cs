using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Parts
{
    public class Spell : IEventSection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SpellSchool School { get; set; }
    }
}

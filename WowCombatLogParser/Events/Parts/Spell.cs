using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Events;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Parts
{
    public class Spell : EventSection
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SpellSchool School { get; set; }
    }
}

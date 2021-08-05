using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Parts
{
    public class SpellSchool : EventSection
    {
        public long Type { get; set; }
        public SpellSchools School => (SpellSchools)Type;        
    }   
}

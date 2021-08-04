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
        public SpellSchool(byte type)
        {
            Type = type;
        }
        
        public byte Type { get; set; }
    }
}

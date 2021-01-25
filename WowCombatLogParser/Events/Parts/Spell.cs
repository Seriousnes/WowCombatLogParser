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
        [FieldOrder(1)]
        public int Id { get; set; }
        [FieldOrder(2)] 
        public string Name { get; set; }
        [FieldOrder(3)] 
        public long School { get; set; }       
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Parts
{
    public class Unit : EventSection
    {
        [FieldOrder(1)]
        public string Guid { get; set; }

        [FieldOrder(2)]
        public string Name { get; set; }
        
        [FieldOrder(3)]        
        public long Flags { get; set; }
        
        [FieldOrder(4)]
        public long RaidFlags { get; set; }
    }
}

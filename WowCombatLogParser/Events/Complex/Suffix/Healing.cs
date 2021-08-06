using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex
{
    [Suffix("_HEAL")]
    public class Healing : IEventSection
    {
        public int Amount { get; set; }
        public int Overhealing { get; set; }
        public int Absorbed { get; set; }
        public bool Critical { get; set; }
    }
}

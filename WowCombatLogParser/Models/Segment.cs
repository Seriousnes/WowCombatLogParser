using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWCombatLogParser.Models
{
    public class Segment
    {
        private Dictionary<string, Combatant> _units = new();
        private List<CombatLogEvent> _events = new();        
    }
}

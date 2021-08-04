using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Events.Parts;
using WoWCombatLogParser.Events.Simple;

namespace WoWCombatLogParser.Models
{
    public class Combatant : Actor
    {
        public CombatantInfo CombatantInfo { get; set; }
    }
}

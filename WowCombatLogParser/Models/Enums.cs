using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWCombatLogParser.Models
{
    public enum Reaction
    {
        Neutral,
        Friendly,
        Hostile
    }
    
    public enum UnitType
    {
        Player,
        Pet,
        NPC,
    }
}

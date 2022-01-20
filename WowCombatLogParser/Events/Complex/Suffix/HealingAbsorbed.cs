using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Events.Parts;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex
{
    [Suffix("_HEAL_ABSORBED")]
    [DebuggerDisplay("{ExtraUnit} {ExtraSpell} {ExtraAmount}")]
    public class HealingAbsorbed : EventSection
    {
        public Unit ExtraUnit { get; set; } = new();
        public Ability ExtraSpell { get; set; } = new();
        public int ExtraAmount { get; set; }
    }
}

using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    [Suffix("_HEAL_ABSORBED")]
    [DebuggerDisplay("{ExtraUnit} {ExtraSpell} {ExtraAmount}")]
    public class HealingAbsorbed
    {
        public Unit ExtraUnit { get; set; } = new Unit();
        public Ability ExtraSpell { get; set; } = new Ability();
        public int ExtraAmount { get; set; }
    }
}

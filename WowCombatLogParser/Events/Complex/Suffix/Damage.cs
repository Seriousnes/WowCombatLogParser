using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Events.Parts;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex.Suffix
{
    [Suffix("_DAMAGE")]
    public class Damage : EventSection
    {
        public decimal Amount { get; set; }
        public decimal Overkill { get; set; }
        public SpellSchool School { get; set; }
        public decimal Resisted { get; set; }
        public decimal Blocked { get; set; }
        public decimal Absorbed { get; set; }
        public bool Critical { get; set; }
        public bool Glancing { get; set; }
        public bool Crushing { get; set; }
        public bool IsOffHand { get; set; }
    }
}

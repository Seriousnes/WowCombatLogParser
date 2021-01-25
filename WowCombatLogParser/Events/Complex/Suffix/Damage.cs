using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex.Suffix
{
    //[Suffix("_DAMAGE")]
    public class Damage : EventSection
    {
        [FieldOrder(1)]
        public decimal Amount { get; set; }
        [FieldOrder(2)]
        public decimal Overkill { get; set; }
        [FieldOrder(3)]
        public long School { get; set; }
        [FieldOrder(4)]
        public decimal Resisted { get; set; }
        [FieldOrder(5)]
        public decimal Blocked { get; set; }
        [FieldOrder(6)]
        public decimal Absorbed { get; set; }
        [FieldOrder(7)]
        public bool Critical { get; set; }
        [FieldOrder(8)]
        public bool Glancing { get; set; }
        [FieldOrder(9)]
        public bool Crushing { get; set; }
        [FieldOrder(10)]
        public bool IsOffHand { get; set; }
    }
}

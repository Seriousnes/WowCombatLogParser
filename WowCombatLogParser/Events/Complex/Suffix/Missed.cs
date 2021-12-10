using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex
{
    [Suffix("_MISSED")]
    [DebuggerDisplay("{MissType} {IsOffHand} {AmountMissed} {Critical}")]
    public class Missed : IEventSection
    {
        public MissType MissType { get; set; }
        public bool IsOffHand { get; set; }
        public decimal AmountMissed { get; set; }
        public bool Critical { get; set; }
    }
}

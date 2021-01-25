using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex.Suffix
{
    [Suffix("_AURA_APPLIED")]
    public class AuraApplied : EventSection
    {
        [FieldOrder(1)]
        public string AuraType { get; set; }
        [FieldOrder(2)]
        public decimal Amount { get; set; }
    }
}

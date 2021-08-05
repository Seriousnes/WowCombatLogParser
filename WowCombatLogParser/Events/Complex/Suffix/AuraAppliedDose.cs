using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex
{
    [Suffix("_AURA_APPLIED_DOSE")]
    public class AuraAppliedDose : IEventSection
    {
        public string AuraType { get; set; }
        public decimal Stacks { get; set; }
    }
}

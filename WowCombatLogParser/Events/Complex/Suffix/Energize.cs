using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex
{
    [Suffix("_ENERGIZE")]
    public class Energize : IEventSection
    {
        public int Amount { get; set; }
        public int OverEnergize { get; set; }
        public PowerType PowerType { get; set; }
        public PowerType AlternatePowerType { get; set; }
    }
}

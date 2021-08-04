using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex.Prefix
{
    [Prefix("ENVIRONMENTAL")]
    public class Environmental : EventSection
    {
        public object EnvironmentalType { get; set; }        
    }
}

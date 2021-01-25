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
        [FieldOrder(1)]
        public object EnvironmentalType { get; set; }        
    }
}

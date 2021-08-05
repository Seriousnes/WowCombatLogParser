using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Simple
{
    [Affix("UNIT_DIED")]
    public class UnitDied : EventSection
    {
    }
}

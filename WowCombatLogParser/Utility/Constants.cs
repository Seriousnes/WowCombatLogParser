using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser
{
    public static class Constants
    {
        public static readonly CombatLogVersion DefaultCombatLogVersion = Enum.GetValues(typeof(CombatLogVersion)).Cast<CombatLogVersion>().Max(x => x);
    }
}

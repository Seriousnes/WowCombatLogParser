using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    [Suffix("_EXTRA_ATTACKS")]
    public class ExtraAttacks : EventSection
    {
        public int Amount { get; set; }
    }
}

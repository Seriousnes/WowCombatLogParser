using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    [Prefix("ENVIRONMENTAL")]
    [SuffixAllowed(typeof(Damage))]
    [DebuggerDisplay("{EnvironmentalType}")]
    public class Environmental : EventSection
    {
        public object EnvironmentalType { get; set; }
    }
}

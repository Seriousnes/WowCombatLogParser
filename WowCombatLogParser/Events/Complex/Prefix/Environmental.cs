using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex
{
    [Prefix("ENVIRONMENTAL")]
    [SuffixAllowed(typeof(Damage))]
    [DebuggerDisplay("{EnvironmentalType}")] 
    public class Environmental : Part
    {
        public object EnvironmentalType { get; set; }
    }
}

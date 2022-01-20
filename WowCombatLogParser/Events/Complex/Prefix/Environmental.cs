using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex
{
    [Prefix("ENVIRONMENTAL")]
    [SuffixAllowed(typeof(Damage))]
    [DebuggerDisplay("{EnvironmentalType}")] 
    public class Environmental : EventSection
    {
        public object EnvironmentalType { get; set; }
    }
}

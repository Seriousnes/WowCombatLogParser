using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex
{
    [Prefix("ENVIRONMENTAL")]
    [SuffixAllowed(typeof(Damage))]
    [DebuggerDisplay("{EnvironmentalType}")] 
    public class Environmental : IEventSection
    {
        public object EnvironmentalType { get; set; }
    }
}

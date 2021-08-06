using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex
{
    [Prefix("ENVIRONMENTAL")]
    [SuffixAllowed(typeof(Damage))]
    public class Environmental : IEventSection
    {
        public object EnvironmentalType { get; set; }
    }
}

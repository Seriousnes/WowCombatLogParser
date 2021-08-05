using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events.Complex
{
    [Prefix("ENVIRONMENTAL")]
    public class Environmental : IEventSection
    {
        public object EnvironmentalType { get; set; }
    }
}

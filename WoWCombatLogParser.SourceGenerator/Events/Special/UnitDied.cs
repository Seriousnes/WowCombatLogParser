using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

[Affix("UNIT_DIED")]
public class UnitDied
{
    public WowGuid RecapId { get; set; }
    public bool UnconsciousOnDeath { get; set; }
    public object UnusedField1 { get; set; }
    public object UnusedField2 { get; set; }
    public Actor Unit { get; set; } = new Actor();
}

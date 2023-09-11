using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

[Suffix("_EXTRA_ATTACKS")]
public class ExtraAttacks : CombagLogEventComponent
{
    public int Amount { get; set; }
}

using WoWCombatLogParser.Common.Events;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Special;

[Affix("ENVIRONMENTAL_DAMAGE")]
public class EnvironmentalDamage : CombatLogEventComponent, IDamage, IAdvancedLoggingDetails
{
    public Actor Source { get; set; } = new();
    public Actor Destination { get; set; } = new();
    public AdvancedLoggingDetails AdvancedLoggingDetails { get; } = new();
    public EnvironmentalType EnvironmentalType { get; set; }
    public decimal Amount { get; set; }
    public decimal UnmitigatedAmount { get; set; }
    public bool IsOverkill { get; set; }
    public SpellSchool School { get; set; }
    public decimal Resisted { get; set; }
    public decimal Blocked { get; set; }
    public decimal Absorbed { get; set; }
    public bool Critical { get; set; }
    public bool Crushing { get; set; }
    public bool IsOffHand { get; set; }
}

using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

[Suffix("_DAMAGE")]
[DebuggerDisplay("{Amount} {UnmitigatedAmount} {IsOverkill} {School} {Resisted} {Blocked} {Absorbed} {Critical} {Crushing} {IsOffHand}")]
public class Damage : AdvancedLoggingDetailsBase, IDamage, IAdvancedLoggingDetails
{
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


/// <remarks>
/// Inherited events populate properties in order of inheritance. If this CombatLogEventComponent inherits from Damage, it will populate the Damage properties before the advanced logging properties
/// </remarks>
[Suffix("_DAMAGE_LANDED")]
[DebuggerDisplay("{Amount} {UnmitigatedAmount} {IsOverkill} {School} {Resisted} {Blocked} {Absorbed} {Critical} {Crushing} {IsOffHand}")]
public class DamageLanded : AdvancedLoggingDetailsBase, IDamage, IDamageOrHealing, IAdvancedLoggingDetails
{
    public decimal UnmitigatedAmount { get; set; }
    public decimal IsOverkill { get; set; }
    public SpellSchool School { get; set; }
    public decimal Resisted { get; set; }
    public decimal Blocked { get; set; }
    public decimal Absorbed { get; set; }
    public bool Critical { get; set; }
    public bool Crushing { get; set; }
    public bool IsOffHand { get; set; }
}

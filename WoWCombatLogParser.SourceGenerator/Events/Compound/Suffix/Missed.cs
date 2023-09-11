using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

[Suffix("_MISSED")]
[DebuggerDisplay("{MissType} {IsOffHand} {AmountMissed} {Critical}")]
public class Missed : CombagLogEventComponent
{
    public MissType MissType { get; set; }
    public bool IsOffHand { get; set; }
    public decimal AmountMissed { get; set; }
    public bool Critical { get; set; }
}

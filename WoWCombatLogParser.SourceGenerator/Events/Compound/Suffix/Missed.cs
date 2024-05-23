using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

[Suffix("_MISSED")]
[DebuggerDisplay("{MissType} {IsOffHand} {AmountMissed} {Critical}")]
public class Missed : CombatLogEventComponent
{
    public MissType MissType { get; set; }
    [Optional]
    public bool IsOffHand { get; set; }
    [Optional]
    public decimal AmountMissed { get; set; }
    [Optional]
    public bool Critical { get; set; }
}

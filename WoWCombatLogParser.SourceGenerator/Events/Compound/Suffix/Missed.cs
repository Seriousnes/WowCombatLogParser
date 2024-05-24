using System.Diagnostics;
using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;

[Suffix("_MISSED")]
[DebuggerDisplay("{MissType} {IsOffHand} {AmountMissed} {Critical}")]
internal class Missed : CombatLogEventComponent
{
    public MissType MissType { get; set; }
    [Optional]
    public bool IsOffHand { get; set; }
    [Optional]
    public decimal AmountMissed { get; set; }
    [Optional]
    public bool Critical { get; set; }
}

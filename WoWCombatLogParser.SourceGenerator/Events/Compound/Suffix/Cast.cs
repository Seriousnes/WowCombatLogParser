using System.Diagnostics;
using WoWCombatLogParser.Models;
using WoWCombatLogParser.SourceGenerator.Models;

namespace WoWCombatLogParser.SourceGenerator.Events.Compound.Suffix;

internal abstract class Cast : CombatLogEventComponent
{
}

[Suffix("_CAST_START")]
[DebuggerDisplay("Started")]
internal class CastStart : Cast, ICast
{
}

[Suffix("_CAST_SUCCESS")]
[DebuggerDisplay("Success")]
internal class CastSuccess : Cast, ICast
{
}

[Suffix("_CAST_FAILED")]
[DebuggerDisplay("Failed ({FailedType})")]
internal class CastFailed : Cast, ICast
{
    public string FailedType { get; set; }
}

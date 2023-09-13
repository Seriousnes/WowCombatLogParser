using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

public abstract class Cast : CombatLogEventComponent
{
}

[Suffix("_CAST_START")]
[DebuggerDisplay("Started")]
public class CastStart : Cast, ICast
{
}

[Suffix("_CAST_SUCCESS")]
[DebuggerDisplay("Success")]
public class CastSuccess : Cast, ICast
{
}

[Suffix("_CAST_FAILED")]
[DebuggerDisplay("Failed ({FailedType})")]
public class CastFailed : Cast, ICast
{
    public string FailedType { get; set; }
}

using System.Diagnostics;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events
{
    public abstract class Cast : EventSection
    {
    }

    [Suffix("_CAST_START")]
    [DebuggerDisplay("Started")]
    public class CastStart : Cast
    {
    }

    [Suffix("_CAST_SUCCESS")]
    [DebuggerDisplay("Success")]
    public class CastSuccess : Cast
    {
    }

    [Suffix("_CAST_FAILED")]
    [DebuggerDisplay("Failed ({FailedType})")]
    public class CastFailed : Cast
    {
        public string FailedType { get; set; }
    }
}

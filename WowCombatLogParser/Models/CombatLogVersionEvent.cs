using System;
using System.Text.RegularExpressions;

namespace WoWCombatLogParser;

public partial class CombatLogVersionEvent : CombatLogEvent
{
    private static readonly Regex _eventTypeExpr = eventTypeExpr();

    public CombatLogVersionEvent(string line) : this()
    {
        var m = _eventTypeExpr.Match(line).Groups;
        Timestamp = GetValue<DateTime>(m["timestamp"].Value);
        Version = GetValue<CombatLogVersion>(m["version"].Value);
        AdvancedLogEnabled = GetValue<bool>(m["advancedlogenabled"].Value);
        BuildVersion = m["buildversion"].Value;
        ProjectId = GetValue<int>(m["projectid"].Value);
    }

    public CombatLogVersionEvent(CombatLogVersion version) : this()
    {
        Version = version;
    }

    [GeneratedRegex(@"(?<timestamp>.*?)\s{2}COMBAT_LOG_VERSION,(?<version>.*?),ADVANCED_LOG_ENABLED,(?<advancedlogenabled>.*?),BUILD_VERSION,(?<buildversion>.*?),PROJECT_ID,(?<projectid>.*)", RegexOptions.Compiled)]
    private static partial Regex eventTypeExpr();
}

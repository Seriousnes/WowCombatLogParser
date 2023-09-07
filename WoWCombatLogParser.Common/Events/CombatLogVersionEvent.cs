﻿using System;
using System.Text.RegularExpressions;
using WoWCombatLogParser.Common.Models;
using WoWCombatLogParser.Common.Utility;

namespace WoWCombatLogParser.Common.Events;

[Affix("COMBAT_LOG_VERSION")]
public class CombatLogVersionEvent : CombatLogEvent
{
    private static readonly Regex _eventTypeExpr = new(@"(?<timestamp>.*?)\s{2}COMBAT_LOG_VERSION,(?<version>.*?),ADVANCED_LOG_ENABLED,(?<advancedlogenabled>.*?),BUILD_VERSION,(?<buildversion>.*?),PROJECT_ID,(?<projectid>.*)", RegexOptions.Compiled);
 
    public CombatLogVersionEvent() : base() { }

    public CombatLogVersionEvent(string line, IApplicationContext applicationContext) : this()
    {
        var m = _eventTypeExpr.Match(line).Groups;
        Timestamp = Conversion.GetValue<DateTime>(m["timestamp"].Value);
        Version = Conversion.GetValue<CombatLogVersion>(m["version"].Value);
        AdvancedLogEnabled = Conversion.GetValue<bool>(m["advancedlogenabled"].Value);
        BuildVersion = m["buildversion"].Value;
        ProjectId = Conversion.GetValue<int>(m["projectid"].Value);
        ApplicationContext = applicationContext;
    }

    public CombatLogVersion Version { get; set; }
    public bool AdvancedLogEnabled { get; set; }
    public string BuildVersion { get; set; }
    public int ProjectId { get; set; }    
}

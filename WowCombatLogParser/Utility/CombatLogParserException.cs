using System;
using System.Reflection;
using static WoWCombatLogParser.IO.CombatLogFieldReader;
using WoWCombatLogParser.IO;
using System.Reflection.Metadata;

namespace WoWCombatLogParser;

public class CombatLogParserException(string eventType, Exception innerException)
    : Exception(innerException.Message, innerException)
{
    public string EventType { get; set; } = eventType;
}
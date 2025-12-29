using System;

namespace WoWCombatLogParser;

public class CombatLogParserException(string eventType, Exception innerException)
    : Exception(innerException.Message, innerException)
{
    public string EventType { get; set; } = eventType;
}
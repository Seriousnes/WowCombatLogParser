using System;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Events;

public abstract class BaseCombatLogEvent : CombatLogEventComponent, ICombatLogEvent
{
    private static int _count = 0;

    public BaseCombatLogEvent()
    {
        Id = _count++;
    }

    [SourceGenerator.Models.NonData]
    public int Id { get; private set; }
    public DateTime Timestamp { get; set; }
    [SourceGenerator.Models.NonData]
    public string EventName => GetType().Name;
}
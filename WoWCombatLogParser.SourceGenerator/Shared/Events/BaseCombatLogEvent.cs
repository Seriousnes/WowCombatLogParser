using System;

namespace WoWCombatLogParser;

public abstract class BaseCombatLogEvent : CombatLogEventComponent, ICombatLogEvent
{
    private static int _count = 0;

    public BaseCombatLogEvent()
    {
        Id = _count++;
    }

    [NonData]
    public int Id { get; private set; }
    public DateTime Timestamp { get; set; }
    [NonData]
    public string EventName => GetType().Name;
}
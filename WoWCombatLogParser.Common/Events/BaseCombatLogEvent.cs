using System;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

public abstract class BaseCombatLogEvent : CombatLogEventComponent, ICombatLogEvent
{
    private static int _count = 0;

    public BaseCombatLogEvent()
    {
        Id = ++_count;
    }

    [NonData]
    public int Id { get; init; }
    public DateTime Timestamp { get; set; }
    [NonData]
    public virtual string Name => GetType().Name;    
}
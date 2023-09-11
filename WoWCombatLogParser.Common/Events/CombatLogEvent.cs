using System;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

public abstract class CombatLogEvent : CombagLogEventComponent, ICombatLogEvent
{
    private static int _count = 0;

    public CombatLogEvent()
    {
        Id = ++_count;
    }

    [NonData]
    public int Id { get; init; }
    public DateTime Timestamp { get; set; }
    [NonData]
    public virtual string CombagLogEventComponent => GetType().Name;
    [NonData]
    public IFight Encounter { get; set; }
    [NonData]
    public IApplicationContext ApplicationContext { get; set; }
}
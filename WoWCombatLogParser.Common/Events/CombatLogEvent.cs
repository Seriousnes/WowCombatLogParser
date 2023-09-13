using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Events;

public abstract class CombatLogEvent : CombatLogEventComponent, ICombatLogEvent
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
    public virtual string Name => GetType().Name;    
}
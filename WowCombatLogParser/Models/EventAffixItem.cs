using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WoWCombatLogParser.Models;

internal class EventAffixItem
{
    public EventAffixItem(Type type)
    {
        EventType = type;
    }

    public Type EventType { get; }
    public string Name => Affix != null ? Affix.Value : EventType.Name;
    public DiscriminatorAttribute Affix => EventType.GetCustomAttribute<DiscriminatorAttribute>()!;
}

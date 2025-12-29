using System;
using System.Reflection;

namespace WoWCombatLogParser;

internal class EventAffixItem(Type type)
{
    public Type EventType { get; } = type;
    public string Name => Affix != null ? Affix.Value : EventType.Name;
    public DiscriminatorAttribute Affix => EventType.GetCustomAttribute<DiscriminatorAttribute>()!;
}

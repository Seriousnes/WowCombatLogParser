using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WoWCombatLogParser.SourceGenerator.Models;

internal class EventAffixItem(Type type)
{
    public Type EventType { get; } = type;
    public AffixAttribute Affix => EventType.GetCustomAttribute<AffixAttribute>();
    public bool IsPrefix => Affix is PrefixAttribute;
    public bool IsSuffix => Affix is SuffixAttribute;
    public bool HasRestrictedSuffixes => RestrictedSuffixes?.Count() > 0;

    public IEnumerable<EventAffixItem> RestrictedSuffixes => EventType
            .GetCustomAttributes(typeof(SuffixAllowedAttribute))
            .Cast<SuffixAllowedAttribute>()
            .SelectMany(i => i.Suffixes)
            .Select(i => new EventAffixItem(i));

    public bool CheckSuffixIsAllowed(Type type)
    {
        return EventType
            .GetCustomAttributes(typeof(SuffixNotAllowedAttribute))
            .Cast<SuffixNotAllowedAttribute>()
            .SelectMany(i => i.Suffixes)
            .Contains(type);
    }
}

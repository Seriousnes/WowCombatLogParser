using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WoWCombatLogParser.Utility;

namespace WoWCombatLogParser.Models
{
    [DebuggerDisplay("{DebuggerValue}")]
    public class EventAffixItem
    {
        private string DebuggerValue => $"{EventType.Name} ({(IsSpecial ? "Special" : IsPrefix ? "Prefix" : "Suffix")})";

        public EventAffixItem(Type type)
        {
            EventType = type;
        }

        public Type EventType { get; }
        public AffixAttribute Affix => EventType.GetCustomAttribute<AffixAttribute>();
        public bool IsSpecial => !IsPrefix && !IsSuffix;
        public bool IsPrefix => Affix is PrefixAttribute;
        public bool IsSuffix => Affix is SuffixAttribute;
        public bool HasRestrictedSuffixes => RestrictedSuffixes?.Count() > 0;
        
        public (string name, Type constructorDefinition, Type[] constructorTypeParams) GetSpecialConstructorTypeDefinition()
        {
            if (IsSpecial)
            {
                return EventType switch
                {
                    var e when e.In(typeof(DamageSplit), typeof(DamageShield)) => (Affix.Name, typeof(CombatLogEvent<,>), new[] { typeof(Spell), typeof(Damage) }),
                    var e when e == typeof(DamageShieldMissed) => (Affix.Name, typeof(CombatLogEvent<,>), new[] { typeof(Spell), typeof(Missed) }),
                    _ => (Affix.Name, typeof(CombatLogEvent<>), new[] { EventType })
                };
            }

            return (null, null, null);
        }

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
}

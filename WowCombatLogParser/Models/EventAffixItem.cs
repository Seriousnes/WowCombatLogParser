using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WoWCombatLogParser.Models
{
    [DebuggerDisplay("{DebuggerValue}")]
    class EventAffixItem
    {
        private string DebuggerValue => $"{EventType.Name} ({(IsSimple ? "Special" : IsPrefix ? "Prefix" : "Suffix")})"; 
        
        public EventAffixItem(Type type)
        {
            EventType = type;
        }

        public Type EventType { get; }
        public AffixAttribute Affix => EventType.GetCustomAttribute<AffixAttribute>();
        public bool IsSimple => !IsPrefix && !IsSuffix;
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
                .Any(i => i != type);
        }
    }
}

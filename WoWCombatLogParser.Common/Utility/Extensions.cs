using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Events;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Utility
{
    public static class Extensions
    {
        public static bool MoveBy<T>(this IEnumerator<T> enumerator, int steps = 1)
        {
            var moveResult = true;
            while (moveResult && steps > 0)
            {
                moveResult = enumerator.MoveNext();
                steps--;
            }
            return moveResult;
        }

        public static void Forget(this Task task)
        {
        }

        public static bool In<T>(this T obj, params T[] objects)
        {
            return objects.Contains(obj);
        }

        public static List<PropertyInfo> GetTypePropertyInfo(this Type type)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(i => i.GetCustomAttribute<NonDataAttribute>() == null && (i.PropertyType.IsSubclassOf(typeof(EventSection)) || i.CanWrite))
                .OrderBy(i => i.DeclaringType == type)
                .ToList();
            return properties;
        }

        public static bool Is(this SpellSchool spellSchool, params SpellSchool[] spellSchools) => spellSchools.CombineSpellSchools() == spellSchool;
        public static bool Is(this SpellSchool spellSchool, IEnumerable<SpellSchool> spellSchools) => spellSchool.Is(spellSchools.ToArray());
        public static SpellSchool CombineSpellSchools(this IEnumerable<SpellSchool> spellSchools) => spellSchools.Aggregate(SpellSchool.None, (result, s) => result |= s);

        public static (bool Success, IEnumerator<IField> Enumerator, bool EndOfParent, bool Dispose) GetEnumeratorForProperty(this IEnumerator<IField> data)
        {
            if (data.Current is GroupField groupData)
            {
                if (groupData.Children.Count > 0)
                {
                    var enumerator = groupData.Children.GetEnumerator();
                    return (enumerator.MoveNext() && enumerator.Current != null, enumerator, !data.MoveNext(), true);
                }
                else
                {
                    return (true, null, !data.MoveNext(), false);
                }                
            }

            return (true, data, false, false);
        }

        public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
                return true;
            }

            return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WoWCombatLogParser.IO;

namespace WoWCombatLogParser.Utility
{
    public static class Extensions
    {
        public static bool MoveBy<T>(this IEnumerator<T> enumerator, int steps = 0)
        {
            var moveResult = true;
            while (moveResult && steps >= 0)
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

        public static (bool Success, IEnumerator<IField> Enumerator, bool EndOfParent, bool Dispose) GetEnumeratorForProperty(this IEnumerator<IField> data)
        {
            if (data.Current is GroupField groupData)
            {
                var enumerator = groupData.Children.GetEnumerator();
                return (enumerator.MoveNext(), enumerator, !data.MoveNext(), true);
            }

            return (true, data, false, false);
        }
    }
}

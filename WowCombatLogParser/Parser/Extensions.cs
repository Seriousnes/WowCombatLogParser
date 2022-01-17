using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WoWCombatLogParser.Events;
using WoWCombatLogParser.Models;

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
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(i => i.GetCustomAttribute<NonDataAttribute>() == null && (i.PropertyType.IsSubclassOf(typeof(Part)) || i.CanWrite))                
                .OrderBy(i => i.DeclaringType == type)
                .ToList(); ;
        }

        public static string GetDescription(this Enum element)
        {
            var type = element.GetType();
            var memberInfo = type.GetMember(element.ToString());

            if (memberInfo.Length > 0)
            {
                var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attributes.Length > 0)
                {
                    return ((DescriptionAttribute)attributes[0]).Description;
                }
            }
            return element.ToString();
        }

        public static object FromDescription(string value, Type type)
        {
            foreach (Enum @enum in Enum.GetValues(type))
            {
                if (@enum.GetDescription().Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    return @enum;
                }
            }

            throw new ArgumentException($"{value} isn't a member of {type.Name}");
        }
    }
}

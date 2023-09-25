using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WoWCombatLogParser.Common.Events;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Utility;

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
            .Where(i => !i.HasCustomAttribute<NonDataAttribute>() && (i.PropertyType.IsSubclassOf(typeof(CombatLogEventComponent)) || i.CanWrite))
            .OrderBy(i => i.DeclaringType == type)
            .ToList();
        return properties;
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

    public static bool HasCustomAttribute<T>(this Type type) where T : Attribute => type.GetCustomAttribute<T>() != null;
    public static bool HasCustomAttribute<T>(this PropertyInfo prop) where T : Attribute => prop.GetCustomAttribute<T>() != null;  
    public static bool IsGenericList(this PropertyInfo prop)
    {
        var type = prop.PropertyType;
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
    }

    public static Type GetGenericListType(this PropertyInfo prop)
    {
        return prop.PropertyType.GetGenericArguments()[0];
    }
}

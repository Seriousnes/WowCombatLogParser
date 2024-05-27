using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WoWCombatLogParser;

public static class Extensions
{
    public static List<PropertyInfo> GetTypePropertyInfo(this Type type)
    {
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(i => !i.HasCustomAttribute<NonDataAttribute>() && (i.PropertyType.IsSubclassOf(typeof(CombatLogEventComponent)) || i.CanWrite))
            .OrderBy(i => i.DeclaringType == type)
            .ToList();
        return properties;
    }

    public static bool HasCustomAttribute<T>(this PropertyInfo prop) where T : Attribute => prop.GetCustomAttribute<T>() != null;
}

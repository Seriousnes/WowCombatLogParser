using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WoWCombatLogParser.Events;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Utilities
{
    public static class Extensions
    {
        #region Helpers
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
        #endregion

        #region IEventSection
        public static void Parse(this IEventSection @event, IEnumerator<string> data)
        {
            //var properties = @event.GetType()
            //    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            //    .Where(i => i.GetCustomAttribute<NonDataAttribute>() == null)
            //    .OrderBy(i => i.DeclaringType == @event.GetType())
            //    .ToList();
            @event.ParseEventProperties(data, EventGenerator.GetClassMap(@event.GetType()));
        }

        public static void ParseEventProperties(this IEventSection @event, IEnumerator<string> data, IList<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                var columnsToSkip = property.GetCustomAttribute<OffsetAttribute>()?.Value ?? 0;
                if (typeof(IEventSection).IsAssignableFrom(property.PropertyType))
                {
                    var prop = (IEventSection)property.GetValue(@event);
                    if (data.MoveBy(columnsToSkip - 1))
                    {
                        prop.Parse(data);
                    }
                }
                else
                {
                    if (!property.CanWrite) continue;
                    if (data.MoveBy(columnsToSkip))
                    {
                        property.SetValue(@event, Conversion.GetValue(data.Current, property.PropertyType));
                    }
                }
            }
        }
        #endregion

        #region Enums
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
        #endregion
    }
}

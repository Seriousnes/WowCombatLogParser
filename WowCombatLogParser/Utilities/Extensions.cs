using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WoWCombatLogParser.Events;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Utilities
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

        public static void Parse(this IEventSection @event, IEnumerator<string> data)
        {
            var properties = @event.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .OrderBy(i => i.DeclaringType == @event.GetType())
                .ToList();
            @event.ParseEventProperties(data, properties);
        }

        public static void ParseEventProperties(this IEventSection @event, IEnumerator<string> data, IList<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                if (typeof(IEventSection).IsAssignableFrom(property.PropertyType))
                {
                    var prop = (IEventSection)property.GetValue(@event);
                    prop.Parse(data);
                }
                else
                {
                    if (!property.CanWrite) continue;
                    var method = typeof(Conversions).GetMethod(nameof(Conversions.GetValue));
                    var generic = method.MakeGenericMethod(property.PropertyType);
                    var columnsToSkip = property.GetCustomAttribute<OffsetAttribute>()?.Value ?? 0;

                    if (data.MoveBy(columnsToSkip))
                    {
                        property.SetValue(@event, generic.Invoke(@event, new object[] { data.Current }));
                    }
                }
            }
        }
    }
}

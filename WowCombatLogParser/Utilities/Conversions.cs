using System;
using System.Globalization;

namespace WoWCombatLogParser.Utilities
{
    public static class Conversions
    {
        public static T GetValue<T>(string value)
        {
            if (value == "nil") return default;

            object convertableValue = typeof(T) switch
            {
                var date when date == typeof(DateTime) => DateTime.ParseExact(value, "M/d HH:mm:ss.fff", CultureInfo.InvariantCulture),
                var hex when hex == typeof(long) => Convert.ToInt32(value, value.StartsWith("0x") ? 16 : 10),
                var logical when logical == typeof(bool) => (value == "-1"),
                var str when str == typeof(string) => value.Replace("\"", ""),
                var enumVal when enumVal.IsEnum => Enum.ToObject(typeof(T), Convert.ToInt32(value, value.StartsWith("0x") ? 16 : 10)),
                _ => value,
            };

            return (T)Convert.ChangeType(convertableValue, typeof(T));
        }
    }
}

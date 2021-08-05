using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoWCombatLogParser
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
                _ => value,
            };
            return (T)Convert.ChangeType(convertableValue, typeof(T));
        }
    }
}

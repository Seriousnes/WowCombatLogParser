using System;
using System.Collections.Generic;
using System.Globalization;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Utilities
{
    public static class Conversion
    {
        private static readonly Dictionary<Type, Func<string, object>> _convertableTypes = new()
        {
            { typeof(WowGuid), value => new WowGuid(value) },
            { typeof(DateTime), value => DateTime.ParseExact(value, "M/d HH:mm:ss.fff", CultureInfo.InvariantCulture) },
            { typeof(long), value => ConvertToInt(value) },
            { typeof(bool), value => value == "-1" },
            { typeof(string), value => value.Replace("\"", "") }
        };

        public static object GetValue(string value, Type type)
        {
            if (value == "nil") return default;

            if (_convertableTypes.ContainsKey(type))
            {
                return _convertableTypes[type](value);
            }
            else if (type.IsEnum)
            {
                return ConvertToEnum(value, type);
            }

            return Convert.ChangeType(value, type);
        }

        private static int ConvertToInt(string value) => Convert.ToInt32(value, value.StartsWith("0x") ? 16 : 10);

        private static object ConvertToEnum(string value, Type type)
        {
            try
            {
                return Enum.ToObject(type, ConvertToInt(value));
            }
            catch (FormatException)
            {
                return Extensions.FromDescription(value, type);
            }                
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Utilities
{
    public static class Conversion
    {
        private static readonly Regex isNumber = new Regex(@"^([0-9]+|0x[0-9a-f]+)$", RegexOptions.Compiled);
        private static readonly Dictionary<Type, Func<string, object>> _convertableTypes = new()
        {
            { typeof(WowGuid), value => new WowGuid(value) },
            { typeof(DateTime), value => DateTime.ParseExact(value, "M/d HH:mm:ss.fff", CultureInfo.InvariantCulture) },
            { typeof(decimal), value => decimal.Parse(value, CultureInfo.InvariantCulture) },
            { typeof(long), value => ConvertToInt(value) },
            { typeof(bool), value => value == "-1" },
            { typeof(string), value => value.Replace("\"", "") }
        };

        public static object GetValue(object value, Type type)
        {
            if (value.GetType() == typeof(string) && (string)value == "nil") return default;

            if (_convertableTypes.ContainsKey(type))
            {
                return _convertableTypes[type]((string)value);
            }
            else if (type.IsEnum)
            {
                return ConvertToEnum((string)value, type);
            }

            return Convert.ChangeType(value, type);
        }

        private static int ConvertToInt(string value) => Convert.ToInt32(value, value.StartsWith("0x") ? 16 : 10);

        private static object ConvertToEnum(string value, Type type)
        {
            if (isNumber.IsMatch(value))
            {
                return Enum.ToObject(type, ConvertToInt(value));
            }
            else
            {
                return Extensions.FromDescription(value, type);
            }                
        }
    }
}
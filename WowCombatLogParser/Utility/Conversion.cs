using System;
using System.Globalization;
using System.Text.RegularExpressions;
using WoWCombatLogParser.Common.Models;

namespace WoWCombatLogParser.Common.Utility;

public static class Conversion
{
    private static readonly Regex isNumber = new Regex(@"^([0-9]+|0x[0-9a-f]+)$", RegexOptions.Compiled);
    private static readonly Dictionary<Type, Func<string, object>> _convertableTypes = new()
    {
        { typeof(WowGuid), value => new WowGuid(value) },
        { typeof(DateTime), value => DateTime.ParseExact(value, "M/d HH:mm:ss.fff", CultureInfo.InvariantCulture) },
        { typeof(decimal), value => decimal.Parse(value, CultureInfo.InvariantCulture) },
        { typeof(int), value => ConvertToInt(value) },
        { typeof(bool), value => value != "0" },
        { typeof(string), value => value.Replace("\"", "") },
        { typeof(UnitFlag), value => new UnitFlag(Convert.ToUInt32(value, 16)) },
    };

    public static object GetValue(string value, Type type)
    {
        if (value.In("", "nil")) return default;

        if (_convertableTypes.ContainsKey(type))
        {
            return _convertableTypes[type]((string)value);
        }
        else if (type.IsEnum)
        {
            return ConvertToEnum(value, type);
        }
        var result = Convert.ChangeType(value, type);

        return result;
    }

    public static T GetValue<T>(string value) =>  (T)GetValue(value, typeof(T));
    public static object GetValue(ICombatLogDataField value, Type type) => GetValue(value.ToString(), type);
    private static int ConvertToInt(string value) => Convert.ToInt32(value, value.StartsWith("0x") ? 16 : 10);
    private static object ConvertToEnum(string value, Type type) =>  isNumber.IsMatch(value) ? Enum.ToObject(type, ConvertToInt(value)) : EnumExtensions.FromDescription(value, type);
}
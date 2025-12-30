using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace WoWCombatLogParser.Utility;

internal static partial class Conversion
{
    [GeneratedRegex(@"^([0-9]+|0x[0-9a-f]+)$", RegexOptions.Compiled)]
    private static partial Regex IsNumber();

    private static readonly Regex isNumber = IsNumber();

    private static readonly string[] combatLogTimestampFormats =
    [
        "M/d HH:mm:ss.fff",
        "M/d/yyyy HH:mm:ss.fffff",
    ];

    private static readonly Dictionary<Type, Func<string, object>> _convertableTypes = new()
    {
        { typeof(WowGuid), value => new WowGuid(value) },
        { typeof(DateTime), value => DateTime.ParseExact(value, combatLogTimestampFormats, CultureInfo.InvariantCulture) },
        { typeof(decimal), value => decimal.Parse(value, CultureInfo.InvariantCulture) },
        { typeof(int), value => ConvertToInt(value) },
        { typeof(bool), value => value != "0" },
        { typeof(string), value => value.Replace("\"", "") },
        { typeof(UnitFlag), value => new UnitFlag(Convert.ToUInt32(value, 16)) },
    };
    private static readonly Dictionary<Type, object> _defaultValues = new()
    {
        { typeof(WowGuid), WowGuid.Empty },
        { typeof(DateTime), DateTime.MinValue },
        { typeof(decimal), 0 },
        { typeof(int), 0 },
        { typeof(bool), false },
        { typeof(string), string.Empty },
        { typeof(UnitFlag), new UnitFlag() },
    };

    public static object GetValue(string value, Type type)
    {
        if (value.In("", "nil")) return _defaultValues[type];

        if (_convertableTypes.TryGetValue(type, out var conversionFunc))
        {
            return conversionFunc(value);
        }
        else if (type.IsEnum)
        {
            return ConvertToEnum(value, type);
        }
        var result = Convert.ChangeType(value, type);

        return result;
    }

    public static T GetValue<T>(string value) => (T)GetValue(value, typeof(T));
    public static T GetValue<T>(ICombatLogDataField value) => (T)GetValue(value, typeof(T));
    public static object GetValue(ICombatLogDataField value, Type type) => GetValue(value.ToString()!, type);
    private static int ConvertToInt(string value) => Convert.ToInt32(value, value.StartsWith("0x") ? 16 : 10);
    private static object ConvertToEnum(string value, Type type) => isNumber.IsMatch(value) ? Enum.ToObject(type, ConvertToInt(value)) : type.FromDescription(value);
}
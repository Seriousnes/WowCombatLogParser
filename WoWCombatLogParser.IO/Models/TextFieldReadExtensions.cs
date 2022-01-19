namespace WoWCombatLogParser.IO
{
    internal static class TextFieldReadExtensions
    {
        public static bool In<T>(this T value, params T[] values) => values?.Contains(value) ?? false;
        public static bool NotIn<T>(this T value, params T[] values) => !value.In(values);
    }
}

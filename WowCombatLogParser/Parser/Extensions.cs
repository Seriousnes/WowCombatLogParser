using WoWCombatLogParser.Common.Events;

namespace WoWCombatLogParser.Utility
{
    public static class Extensions
    {
        private static TextFieldReaderOptions options = new() { HasFieldsEnclosedInQuotes = true, Delimiters = new[] { ',' } };
        public static (bool Success, IEnumerator<IField> Enumerator, bool EndOfParent, bool Dispose) GetEnumeratorForProperty(this IEnumerator<IField> data)
        {
            if (data.Current is GroupField groupData)
            {
                var enumerator = groupData.Children.GetEnumerator();
                return (enumerator.MoveNext(), enumerator, !data.MoveNext(), true);
            }

            return (true, data, false, false);
        }

        public static T GetCombatLogEvent<T>(this string line) where T : CombatLogEvent
        {
            return EventGenerator.GetCombatLogEvent<T>(TextFieldReader.ReadFields(line, options));
        }
    }
}

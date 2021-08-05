using System.Collections.Generic;
using WoWCombatLogParser.Events;

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

        public static void Parse(this EventSection @event)
        {

        }
    }
}

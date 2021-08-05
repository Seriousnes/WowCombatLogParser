using System.Collections.Generic;
using System.IO;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser
{
    public class CombatLogParser
    {
        public static IEnumerable<CombatLogEvent> ParseCombatLog(string fileName)
        {
            using var sr = new StreamReader(fileName);
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                var combatLogEvent = EventGenerator.GetCombatLogEvent(line);
                if (combatLogEvent != null)
                {
                    yield return combatLogEvent;
                }
            }
        }

        public static IEnumerable<(CombatLogEvent Event, string Data)> QuickParse(string fileName)
        {
            yield return (null, null);
        }
    }
}

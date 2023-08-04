using System.Linq;
using Xunit.Abstractions;
using WoWCombatLogParser.Common.Models;
using WoWCombatLogParser.Common.Events;
using System.Resources;
using WoWCombatLogParser.Models;

namespace WoWCombatLogParser.Tests
{
    public class CombatLogParsingTestBase : ApplicationContext
    {
        internal readonly ITestOutputHelper output;        

        public CombatLogParsingTestBase(ITestOutputHelper output, CombatLogVersion combatLogVersion)
        {
            CombatLogParser = new CombatLogParser();
            EventGenerator = new EventGenerator
            {
                CombatLogVersionEvent = new CombatLogVersionEvent
                {
                    AdvancedLogEnabled = true,
                    Version = combatLogVersion
                }
            };
            this.output = output;
        }

        internal void OutputEncounterSumary(IFight fight)
        {
            output.WriteLine($"Event Summary\n{new string('=', 35)}");
            output.WriteLine(fight.GetDetails().ToString());
            output.WriteLine(new string('-', 35));
            fight.GetEvents()
                .GroupBy(x => x.Event)
                .OrderBy(x => x.Key)
                .ToList()
                .ForEach(x => output.WriteLine($"{x.Key,-25}{x.Count(),10}"));
            output.WriteLine(new string('-', 35));
            output.WriteLine($"{"Count",-11}{fight.GetEvents().Count,24}");
            output.WriteLine($"{new string('=', 35)}\n\n");
        }
    }
}

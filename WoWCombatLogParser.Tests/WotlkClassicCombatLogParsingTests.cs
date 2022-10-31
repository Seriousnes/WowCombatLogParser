using FluentAssertions;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WoWCombatLogParser.Events;
using WoWCombatLogParser.Models;
using WoWCombatLogParser.Events;
using WoWCombatLogParser.Models;
using WoWCombatLogParser.Utility;
using Xunit;
using Xunit.Abstractions;
using static WoWCombatLogParser.CombatLogParser;


namespace WoWCombatLogParser.Tests
{
    public class WotlkClassicCombatLogParsingTests : CombatLogParsingTestBase
    {
        public WotlkClassicCombatLogParsingTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Test_FullRaidCombatLog()
        {
            var parser = new CombatLogParser(@"TestLogs/WotlkClassic/Naxxramas.txt");
            var encounters = parser.Scan().ToList();

            foreach (var encounter in encounters)
            {
                encounter.Parse();
            }

            //Parallel.ForEachAsync(encounters, async (x, _) => await x.ParseAsync()).Wait();

            encounters.Should().NotBeNull().And.HaveCountGreaterThan(1);
            encounters.ForEach(e => OutputEncounterSumary(e));
        }
    }
}

using FluentAssertions;
using System.Linq;
using WoWCombatLogParser.Common.Models;
using Xunit;
using Xunit.Abstractions;


namespace WoWCombatLogParser.Tests
{
    public class WotlkClassicCombatLogParsingTests : CombatLogParsingTestBase
    {
        public WotlkClassicCombatLogParsingTests(ITestOutputHelper output) : base(output, CombatLogVersion.Wotlk)
        {            
        }

        [Fact]
        public void Test_FullRaidCombatLog()
        {
            CombatLogParser.Filename = @"TestLogs/WotlkClassic/Naxxramas.txt";
            var encounters = CombatLogParser.Scan().ToList();
            CombatLogParser.ParseAsync(encounters).Wait();
            encounters.Should().NotBeNull().And.HaveCountGreaterThan(1);
            encounters.ForEach(e => OutputEncounterSumary(e));
        }
    }
}

using WoWCombatLogParser.Models;
using Xunit;
using Xunit.Abstractions;


namespace WoWCombatLogParser.Tests;

public class WotlkClassicCombatLogParsingTests : CombatLogParsingTestBase
{
    public WotlkClassicCombatLogParsingTests(ITestOutputHelper output) : base(output, CombatLogVersion.Wotlk)
    {            
    }

    [Fact]
    public void Test_FullRaidCombatLog()
    {
        //CombatLogParser.Filename = @"TestLogs/WotlkClassic/Naxxramas.txt";
        //var encounters = CombatLogParser.Scan().ToList();
        //encounters.Should().NotBeNull().And.HaveCountGreaterThan(1);
        //encounters.ForEach(e => OutputEncounterSumary(e));
    }

    [Fact]
    public void Test_FullRaidCombatLogAsync()
    {
        //CombatLogParser.Filename = @"TestLogs/WotlkClassic/Naxxramas.txt";
        //var encounters = CombatLogParser.Scan().ToList();
        //encounters.Should().NotBeNull().And.HaveCountGreaterThan(1);
        //encounters.ForEach(e => OutputEncounterSumary(e));
    }
}

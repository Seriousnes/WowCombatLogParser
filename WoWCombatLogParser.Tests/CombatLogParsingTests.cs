using System.Linq;
using WoWCombatLogParser.Models;
using Xunit;
using Xunit.Abstractions;
using static WoWCombatLogParser.CombatLogParser;

namespace WoWCombatLogParser.Tests
{
    public class CombatLogParsingTests
    {
        private readonly ITestOutputHelper output;

        public CombatLogParsingTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void TestCombatLogParses()
        {
            var encounters = ParseCombatLogSegments(@"TestLogs/WoWCombatLog.txt").Take(2).ToList();

            Assert.True(encounters[0].Count == 669, "Segment 1 has 669 events");
            Assert.True(encounters[1].Count == 8408, "Segment 2 has 8408 events");
        }

        [Theory]
        [InlineData(@"TestLogs/WoWCombatLog-112821_193218.txt")]
        public void TestLargeCombatLog(string filename)
        {
            var encountersInFile = ParseCombatLogSegments(filename);
            Assert.True(encountersInFile.Any());

            //var bigEncounter = encountersInFile.Skip(1).Take(1).SingleOrDefault();
            //output.WriteLine($"Events: {bigEncounter.Count}");
            encountersInFile
                .ToList()
                .ForEach(e => output.WriteLine($"Events: {e.Count}"));
        }

        [Theory]
        [InlineData(SpellSchool.Elemental, SpellSchool.Nature, SpellSchool.Fire, SpellSchool.Frost)]
        [InlineData(SpellSchool.FlameStrike, SpellSchool.Fire, SpellSchool.Physical)]
        public void TestSpellSchools(SpellSchool expected, params SpellSchool[] spellSchools)
        {
            SpellSchool calculated = SpellSchool.None;
            foreach (var school in spellSchools)
            {
                calculated |= school;
            }

            Assert.True(expected == calculated);
        }
    }
}

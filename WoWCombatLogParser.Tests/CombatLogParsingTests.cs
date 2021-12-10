using System.Linq;
using WoWCombatLogParser.Models;
using Xunit;
using static WoWCombatLogParser.CombatLogParser;

namespace WoWCombatLogParser.Tests
{
    public class CombatLogParsingTests
    {
        [Fact]
        public void TestCombatLogParses()
        {
            var segments = ParseCombatLogSegments(@"TestLogs/WoWCombatLog.txt").Take(2).ToList();

            Assert.True(segments[0].Events.Count == 669, "Segment 1 has 669 events");
            Assert.True(segments[1].Events.Count == 8408, "Segment 2 has 8408 events");
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

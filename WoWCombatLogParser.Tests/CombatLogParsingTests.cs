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

            //var spellDamage = segment.Events.OfType<CombatLogEvent<Spell, Damage>>().ToList();

            Assert.True(segments.Count() > 0);
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

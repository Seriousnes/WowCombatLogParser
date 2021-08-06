using System;
using System.Linq;
using WoWCombatLogParser;
using WoWCombatLogParser.Events.Complex;
using WoWCombatLogParser.Events.Simple;
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
            var events = ParseCombatLog(@"TestLogs/WoWCombatLog.txt").ToList();

            var spellDamage = events.OfType<CombatLogEvent<Spell, Damage>>().ToList();

            Assert.True(events.Count() > 0);
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

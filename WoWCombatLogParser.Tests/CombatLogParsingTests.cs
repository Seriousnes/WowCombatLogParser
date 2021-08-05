using System;
using System.Linq;
using WoWCombatLogParser;
using WoWCombatLogParser.Events.Complex.Prefix;
using WoWCombatLogParser.Events.Complex.Suffix;
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

        [Fact]
        public void TestFieldOrder()
        {
            var obj = new CombatantInfo();
            var fields = obj.GetType().GetProperties();
            Assert.True(fields.Count() > 0);
        }

        [Theory]
        [InlineData(SpellSchools.Elemental, SpellSchools.Nature, SpellSchools.Fire, SpellSchools.Frost)]
        public void TestSpellSchools(SpellSchools expected, params SpellSchools[] spellSchools)
        {
            SpellSchools calculated = SpellSchools.None;
            foreach (var school in spellSchools)
            {
                calculated |= school;
            }

            Assert.True(expected == calculated);
        }
    }    
}

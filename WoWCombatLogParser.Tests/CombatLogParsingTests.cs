using System.Linq;
using WoWCombatLogParser.Models;
using Xunit;
using Xunit.Abstractions;
using static WoWCombatLogParser.CombatLogParser;
using FluentAssertions;

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
        //[InlineData(@"TestLogs/WoWCombatLog-112821_193218.txt")]
        [InlineData(@"TestLogs/SingleFightCombatLog.txt")]
        public void TestCombatLogs(string filename)
        {
            var encountersInFile = ParseCombatLogSegments(filename).ToList();
            Assert.True(encountersInFile.Any());
            encountersInFile.ForEach(e => output.WriteLine($"Events: {e.Count}"));
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

        [Theory]
        [InlineData(0x1148, UnitTypeFlag.Pet, ReactionFlag.Hostile, OwnershipFlag.Player, AffiliationFlag.Outsider)]
        [InlineData(0x1248, UnitTypeFlag.Pet, ReactionFlag.Hostile, OwnershipFlag.Npc, AffiliationFlag.Outsider)]
        [InlineData(0x0548, UnitTypeFlag.Player, ReactionFlag.Hostile, OwnershipFlag.Player, AffiliationFlag.Outsider)]
        public void TestUnitFlags(uint flags, UnitTypeFlag type, ReactionFlag reaction, OwnershipFlag controller, AffiliationFlag affiliation)
        {
            var unitFlags = new UnitFlag(flags);

            unitFlags.UnitType.Should().Be(type);
            unitFlags.Reaction.Should().Be(reaction);
            unitFlags.Ownership.Should().Be(controller);
            unitFlags.Affiliation.Should().Be(affiliation);
        }
    }
}

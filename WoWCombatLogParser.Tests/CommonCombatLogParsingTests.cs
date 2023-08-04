using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using WoWCombatLogParser.Common.Models;
using WoWCombatLogParser.Common.Utility;
using System.Linq;

namespace WoWCombatLogParser.Tests
{
    public class CommonCombatLogParsingTests : CombatLogParsingTestBase
    {
        public CommonCombatLogParsingTests(ITestOutputHelper output) : base(output, Constants.DefaultCombatLogVersion)
        {
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

        [Theory]
        [InlineData(SpellSchool.Elemental, SpellSchool.Nature, SpellSchool.Fire, SpellSchool.Frost)]
        [InlineData(SpellSchool.Inferno, SpellSchool.Fire, SpellSchool.Physical)]
        public void TestSpellSchools(SpellSchool expected, params SpellSchool[] spellSchools)
        {
            Assert.True(expected.Is(spellSchools));
        }

        [Fact]
        public void Test_RegisteredEventHandlers()
        {
            EventGenerator.GetRegisteredEventHandlers().ForEach(x => output.WriteLine(x));
        }

        [Fact]
        public void Test_RegisteredClassMaps()
        {
            EventGenerator.GetRegisteredClassMaps().ForEach(x => output.WriteLine(x));
        }
    }
}

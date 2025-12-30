using FluentAssertions;
using System;
using Xunit;
using Xunit.Abstractions;
using WoWCombatLogParser.Utility;

namespace WoWCombatLogParser.Tests;

public class CommonCombatLogParsingTests(ITestOutputHelper output) : CombatLogParsingTestBase(output)
{
    [Theory]
    [InlineData("12/30 19:23:45.123", 1230000L)]
    [InlineData("12/30/2025 19:23:45.12345", 1234500L)]
    public void TestTimestampPrecision(string input, long expectedFractionTicks)
    {
        var timestamp = Conversion.GetValue<DateTime>(input);

        timestamp.Month.Should().Be(12);
        timestamp.Day.Should().Be(30);
        timestamp.Hour.Should().Be(19);
        timestamp.Minute.Should().Be(23);
        timestamp.Second.Should().Be(45);

        var fractionTicks = timestamp.TimeOfDay.Ticks % TimeSpan.TicksPerSecond;
        fractionTicks.Should().Be(expectedFractionTicks);
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
}

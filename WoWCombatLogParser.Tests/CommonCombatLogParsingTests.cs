using FluentAssertions;
using Xunit;
using Xunit.Abstractions;
using WoWCombatLogParser.Common.Models;
using WoWCombatLogParser.Common.Utility;
using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Net.Mime;

namespace WoWCombatLogParser.Tests;

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

    [Theory]
    [InlineData(@"TestLogs/Dragonflight/WoWCombatLog.txt")]
    public async Task Test_FindPerformanceOnFile(string filename)
    {
        var sw = new Stopwatch();

        using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var sr = new StreamReader(fs);
        var content = sr.ReadToEnd();

        sw.Start();
        var indexes = new List<Line>();
        int i = -1;
        while ((i = content.IndexOf("ENCOUNTER_START", i + 1, StringComparison.Ordinal)) >= 0)
        {
            var e = content.IndexOf(Environment.NewLine, i, StringComparison.Ordinal);
            var t = content[(i-20)..e];
            indexes.Add(new Line { Event = i, End = e, EventText = t });
        }
        sw.Stop();
        output.WriteLine($"Loop with indexOf: {new TimeSpan(sw.ElapsedTicks).TotalMilliseconds}");
    }

    [DebuggerDisplay("{Event} ({Start} - {End}): {EventText}")]
    struct Line
    {
        public int Start => Event - 20;
        public int Event { get; set; }
        public int End { get; set; } 
        public string EventText { get; set; }
        int RangeEnd { get; set; }
    }

    private static void BadCharHeuristic(string str, int size, ref int[] badChar)
    {
        int i;

        for (i = 0; i < 256; i++)
            badChar[i] = -1;

        for (i = 0; i < size; i++)
            badChar[(int)str[i]] = i;
    }
}

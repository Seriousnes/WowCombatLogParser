using FluentAssertions;
using System;
using WoWCombatLogParser.Events;
using WoWCombatLogParser.Models;
using WoWCombatLogParser.Common.Models;
using Xunit;
using Xunit.Abstractions;

namespace WoWCombatLogParser.Tests;

public class ShadowlandsCombatLogParsingTests : CombatLogParsingTestBase
{
    public ShadowlandsCombatLogParsingTests(ITestOutputHelper output) : base(output, CombatLogVersion.Shadowlands)
    {            
    }

    [Theory]
    [InlineData(@"TestLogs/Shadowlands/SingleFightCombatLog.txt", true)]
    public void Test_SingleEncounter(string fileName, bool isAsync)
    {
        //CombatLogParser.Filename = fileName;
        //IFight encounter = CombatLogParser.Scan().First();
        //encounter.Should().NotBeNull().And.BeAssignableTo<Boss>();
        //OutputEncounterSumary(encounter);
    }

    [Fact(Skip = "Used for debugging")]       
    public void Test_FullRaidCombatLog()
    {
        //CombatLogParser.Filename = @"TestLogs/Shadowlands/WoWCombatLog-112821_193218.txt";
        //var encounters = CombatLogParser.Scan().ToList();
        //encounters.Should().NotBeNull().And.HaveCountGreaterThan(1);
        //encounters.ForEach(e => OutputEncounterSumary(e));
    }

    [Fact]
    public void Test_FullRaidCombatLogAsync()
    {
        //CombatLogParser.Filename = @"TestLogs/Shadowlands/WoWCombatLog-112821_193218.txt";
        //var encounters = CombatLogParser.Scan().ToList();
        //encounters.Should().NotBeNull().And.HaveCountGreaterThan(1);
        //encounters.ForEach(e => OutputEncounterSumary(e));
    }

    [Fact]
    public void Test_ScanMultipleFightsSelectingSecond()
    {
        //CombatLogParser.Filename = @"TestLogs/Shadowlands/WoWCombatLog-112821_193218.txt";
        //var encounter = CombatLogParser.Scan().Skip(1).Take(1).SingleOrDefault();
        //encounter.Should().NotBeNull();
        //OutputEncounterSumary(encounter);
    }

    [Theory]
    [InlineData(@"11/28 19:54:13.422  SPELL_DISPEL,Player-3725-0AF257AE,""Naxa - Frostmourne"",0x514,0x0,Player-3725-06B15901,""Svothgos - Frostmourne"",0x514,0x0,4987,""Cleanse"",0x2,357298,""Frozen Binds"",16,DEBUFF")]
    public void Test_SpellDispel(string input)
    {
        var combatLogEvent = EventGenerator.GetCombatLogEvent<SpellDispel>(input);
        // unit name testing
        combatLogEvent.Source.UnitName.Should().Be("Naxa - Frostmourne");
        combatLogEvent.Source.Name.Should().Be("Naxa");
        combatLogEvent.Source.Server.Should().Be("Frostmourne");
        combatLogEvent.Spell.Name.Should().Be("Cleanse");
        combatLogEvent.ExtraSpell.Id.Should().Be(357298);
    }

    [Theory]
    [InlineData(@"11/28 19:36:59.856  ENCOUNTER_END,2431,""Fatescribe Roh - Kalo"",15,14,0,271825", false)]
    [InlineData(@"11/28 19:46:43.635  ENCOUNTER_END,2431,""Fatescribe Roh-Kalo"",15,14,1,404969", true)]
    public void Test_EncounterEnd(string input, bool success)
    {
        var combatLogEvent = EventGenerator.GetCombatLogEvent<EncounterEnd>(input);
        combatLogEvent.Success.Should().Be(success);
    }

    [Theory]
    [InlineData(@"11/28 19:32:28.026  COMBATANT_INFO,Player-3725-09C56D56,1,217,1589,2469,496,0,0,0,450,450,450,60,30,865,865,865,39,358,334,334,334,930,263,(117014,201900,260878,210853,196884,197214,262624),(0,193876,204331,204264),[1,3,[],[(844),(849),(850),(858),(863),(864),(995),(1010),(1837),(1839),(1841),(1842)],[(94,239),(111,226),(93,184),(95,239),(110,239),(98,226)]],[(186341,239,(),(7188,6652,1485,6646),(187319,255)),(186291,239,(),(7188,6652,7575,1485,6646),()),(172327,225,(),(6995,6718,6648,6649,1522),()),(0,0,(),(),()),(186303,239,(6230,0,0),(7188,6652,1485,6646),(187320,255)),(186301,239,(),(7188,6652,1485,6646),(187318,255)),(186307,239,(),(7188,6652,1485,6646),()),(186343,239,(6211,0,0),(7188,6652,1485,6646),(187065,255)),(178767,252,(),(7622,7359,6652,7574,1566,6646),()),(186308,239,(),(7188,6652,1485,6646),()),(186377,233,(6166,0,0),(7189,40,7575,1472,6646),()),(186375,239,(6166,0,0),(7188,6652,7575,1485,6646),()),(186432,226,(),(7189,6652,1472,6646),()),(186423,239,(),(7188,6652,1485,6646),()),(186374,239,(6204,0,0),(7188,6652,1485,6646),()),(186388,239,(6228,5401,0),(7188,6652,1485,6646),()),(186387,239,(6229,5400,0),(7188,6652,1485,6646),()),(0,0,(),(),())],[Player-3725-09C56D56,307185,Player-3725-09C56D56,327709,Player-3725-09C56D56,2645,Player-3725-09D57DD8,1459,Player-3725-09C56D56,355794,Player-3725-09D5AE20,21562,Player-3725-09D7A162,6673],23,0,0,0")]
    public void Test_CombantInfo(string input)
    {
        var combatantInfo = EventGenerator.GetCombatLogEvent<ShadowlandsCombatantInfo>(input);
        combatantInfo.ClassTalents.Should().HaveCount(7);
        combatantInfo.Powers.SoulbindTraits.Should().HaveCount(12);
        combatantInfo.Powers.Conduits.Should().HaveCount(6);
        combatantInfo.EquippedItems.Should().HaveCount(18);
        combatantInfo.InterestingAuras.Should().HaveCount(7);
        Assert.True(combatantInfo.PvPStats is { HonorLevel: 23, Rating: 0, Season: 0, Tier: 0 });
    }

    [Theory]
    [InlineData(@"11/28 19:40:57.094  DAMAGE_SPLIT,Player-3725-0669E64A,""Formid - Frostmourne"",0x514,0x0,Player-3725-09FE7744,""Khalous - Frostmourne"",0x40514,0x0,6940,""Blessing of Sacrifice"",0x2,Player-3725-09FE7744,0000000000000000,67569,86120,2586,472,5346,0,0,9741,10000,0,76.88,-900.65,2001,0.0607,246,1302,0,-1,32,0,0,0,nil,nil,nil")]
    public void Test_DamageSplit(string input)
    {
        var combatLogEvent = EventGenerator.GetCombatLogEvent<DamageSplit>(input);
    }

    [Theory]
    [InlineData(typeof(SpellPeriodicDamage), @"11/28 19:32:36.434  SPELL_PERIODIC_DAMAGE,Creature-0-5047-2450-26923-175730-0000234859,""Fatescribe Roh-Kalo"",0x10a48,0x0,Player-1136-08E79DB6,""Bansky-Gurubashi"",0x512,0x0,353931,""Twist Fate"",0x20,Player-1136-08E79DB6,0000000000000000,50393,54600,2361,327,682,759,17,33,120,0,65.61,-901.65,2001,4.7087,247,4207,7861,-1,32,0,0,1601,nil,nil,nil")]
    [InlineData(typeof(SpellDamage), @"11/28 19:46:43.567  SPELL_DAMAGE,Pet-0-5047-2450-26923-165189-0203600F87,""Gruffhorn"",0x1114,0x0,Creature-0-5047-2450-26923-175730-0000234DDC,""Fatescribe Roh-Kalo"",0x10a48,0x0,83381,""Kill Command"",0x1,Creature-0-5047-2450-26923-175730-0000234DDC,0000000000000000,139,20164970,0,0,1071,0,3,7,100,0,100.86,-931.17,2001,2.9855,63,2245,3021,-1,1,0,0,0,nil,nil,nil")]
    [InlineData(typeof(SpellDamage), @"11/28 19:32:46.738  SPELL_DAMAGE,Player-3725-0BF357DA,""Koriz-Frostmourne"",0x514,0x0,Creature-0-5047-2450-26923-175730-0000234859,""Fatescribe Roh-Kalo"",0x10a48,0x0,285452,""Lava Burst"",0x4,Creature-0-5047-2450-26923-175730-0000234859,0000000000000000,18216931,20164970,0,0,1071,0,3,0,100,0,64.06,-904.28,2001,1.9476,63,8215,3816,-1,4,0,0,0,1,nil,nil")]
    public void Test_DamageSuffix(Type eventType, string input)
    {
        var combatLogEvent = EventGenerator.GetCombatLogEvent<CombatLogEvent>(input);
        combatLogEvent.GetType().Should().Be(eventType);

        switch (combatLogEvent)
        {
            case SpellDamage spellDamage:
                spellDamage.Destination.Name.Should().Be("Fatescribe Roh-Kalo");
                break;
            case SpellPeriodicDamage spellPeriodicDamage:
                spellPeriodicDamage.Source.Name.Should().Be("Fatescribe Roh-Kalo");
                break;
        }
    }
}

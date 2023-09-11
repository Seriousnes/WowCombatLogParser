using FluentAssertions;
using System;
using System.Linq;
using WoWCombatLogParser.Events;
using WoWCombatLogParser.Models;
using WoWCombatLogParser.Common.Events;
using WoWCombatLogParser.Common.Models;
using Xunit;
using Xunit.Abstractions;

namespace WoWCombatLogParser.Tests;

public class DragonflightCombatLogParsingTests : CombatLogParsingTestBase
{
    public DragonflightCombatLogParsingTests(ITestOutputHelper output) : base(output, CombatLogVersion.Dragonflight)
    {            
    }

    [Theory]
    [InlineData(@"TestLogs/Dragonflight/WoWCombatLog.txt", true)]        
    [InlineData(@"TestLogs/Dragonflight/EchoOfNeltharion_Wipe.txt", true)]
    public void Test_SingleEncounter(string fileName, bool isAsync)
    {
        CombatLogParser.Filename = fileName;
        IFight encounter = CombatLogParser.Scan(quickScan: true).First();
        encounter.Should().NotBeNull().And.BeAssignableTo<Boss>();
        var combatants = encounter.GetEvents().OfType<DragonflightCombatantInfo>().ToList();

        OutputEncounterSumary(encounter);
    }


    [Fact(Skip = "Used for debugging")]       
    public void Test_FullRaidCombatLog()
    {
        CombatLogParser.Filename = @"TestLogs/Dragonflight/WoWCombatLog.txt";
        var encounters = CombatLogParser.Scan().ToList();
        encounters.Should().NotBeNull().And.HaveCountGreaterThan(1);
        encounters.ForEach(e => OutputEncounterSumary(e));
    }

    [Theory]
    [InlineData(@"12/14 19:51:23.689  SPELL_CAST_SUCCESS,Pet-0-3766-2522-21131-17252-010354DEAA,""Rivinfaran"",0x1114,0x0,0000000000000000,nil,0x80000000,0x80000000,89753,""Felstorm"",0x1,Pet-0-3766-2522-21131-17252-010354DEAA,Player-3723-0BE31FFD,178936,178936,4186,8372,6124,17893,3,159,200,0,-566.84,292.29,2120,4.9520,371")]
    [InlineData(@"12/14 20:43:17.474  SPELL_HEAL,Player-3725-0A852785,""Nahmateyeah-Frostmourne"",0x514,0x0,Player-3725-0A852785,""Nahmateyeah-Frostmourne"",0x514,0x0,143924,""Leech"",0x1,Player-3725-0A852785,0000000000000000,123114,215762,2756,7768,6311,0,11,26,150,0,-59.24,328.67,2122,4.9520,365,94,94,0,0,nil")]
    [InlineData(@"12/14 20:43:17.685  ENVIRONMENTAL_DAMAGE,0000000000000000,nil,0x80000000,0x80000000,Player-3725-0A852785,""Nahmateyeah-Frostmourne"",0x514,0x0,Player-3725-0A852785,0000000000000000,110938,215762,2756,7768,6311,0,11,26,150,0,-58.55,324.36,2122,4.9520,365,Falling,12176,12176,0,1,0,0,0,nil,nil,nil")]
    [InlineData(@"12/14 20:43:17.855  SPELL_PERIODIC_HEAL,Player-3725-0C164F8F,""Hypocrisies-Frostmourne"",0x512,0x0,Player-3725-0A852785,""Nahmateyeah-Frostmourne"",0x514,0x0,366155,""Reversion"",0x40,Player-3725-0A852785,0000000000000000,115122,215762,2756,7768,6311,0,11,26,150,0,-58.04,321.11,2122,4.9520,365,4184,4184,0,0,nil")]
    [InlineData(@"12/14 20:43:18.360  SPELL_ENERGIZE,Player-3725-0A852785,""Nahmateyeah-Frostmourne"",0x514,0x0,Player-3725-0A852785,""Nahmateyeah-Frostmourne"",0x514,0x0,378777,""Inundate"",0x1,Player-3725-0A852785,0000000000000000,115122,215762,2756,7768,6311,0,11,34,150,0,-57.42,318.18,2122,4.9520,365,8.0000,0.0000,11,150")]
    [InlineData(@"12/14 20:43:18.360  SPELL_CAST_SUCCESS,Player-3725-0A852785,""Nahmateyeah-Frostmourne"",0x514,0x0,0000000000000000,nil,0x80000000,0x80000000,5394,""Healing Stream Totem"",0x8,Player-3725-0A852785,0000000000000000,115122,215762,2756,7768,6311,0,0,50000,50000,4500,-57.42,318.18,2122,4.9520,365")]
    [InlineData(@"11/28 19:54:13.422  SPELL_DISPEL,Player-3725-0AF257AE,""Naxa - Frostmourne"",0x514,0x0,Player-3725-06B15901,""Svothgos - Frostmourne"",0x514,0x0,4987,""Cleanse"",0x2,357298,""Frozen Binds"",16,DEBUFF")]
    [InlineData(@"12/14 19:35:08.435  COMBATANT_INFO,Player-3725-09C56D56,1,1012,5699,9982,2312,0,0,0,2051,2051,2051,135,0,2181,2181,2181,189,2414,822,822,822,3563,263,[(81074,101965,1),(81059,101947,1),(81068,101957,1),(81067,101956,1),(81098,101996,2),(81086,101980,2),(81087,101981,2),(81088,101983,1),(80941,101804,1),(80957,101822,2),(80939,101802,1),(80942,101805,1),(80958,101823,1),(80971,101837,2),(80967,101832,1),(81085,101979,1),(81062,101950,1),(81064,101952,1),(80943,101806,2),(81081,101974,2),(80961,101826,2),(81057,101945,1),(80972,101838,1),(80966,101831,1),(81071,101961,1),(80964,101829,2),(80965,101830,1),(80975,101841,1),(81097,101995,1),(80938,101801,1),(80944,101807,1),(80945,101809,2),(80947,101811,1),(80948,101812,1),(80953,101818,1),(80955,101820,1),(80963,101828,1),(80970,101836,1),(80974,101840,1),(81056,101944,1),(81070,101959,1),(81082,101976,1),(81083,101977,1),(81084,101978,2),(81096,101994,1),(81100,101998,1),(81101,101999,2),(81102,102000,1),(81076,101968,1),(81061,101949,1),(81060,101948,1)],(0,289874,193876,204264),[(193777,372,(),(7977,6652,7936,8816,8835,1594,8767),()),(193809,372,(),(7977,6652,7936,8783,1594,8767),()),(192002,359,(),(6652,1481,8766),()),(0,0,(),(),()),(193801,372,(),(7977,6652,8816,1594,8767),()),(193656,359,(),(7976,6652,7937,8814,1581,8766),()),(192001,376,(),(6652,1485,5858,8767),()),(193685,372,(),(7977,6652,8814,1594,8767),()),(193693,372,(),(7977,6652,7936,8815,1594,8767),()),(191999,369,(),(6652,1472,5864,8766),()),(193671,372,(),(7977,42,7935,1594,8767),(192912,415)),(193804,346,(),(7978,7975,6652,7937),()),(193701,382,(),(8963,7977,6652,9144,1604,8767),()),(200563,372,(),(40,1481,5858,8767),()),(193763,376,(),(8961,7977,6652,8822,8819,9144,1598,8767),()),(190513,386,(0,5401,0),(8836,8840,8902),()),(197947,359,(0,5400,0),(6652,1475,5851,8766),()),(140579,40,(),(),())],[Player-3725-09C56D56,371339,Player-3725-09D57DD8,1459,Player-3725-09C56D56,396092,Player-3725-09D597C5,1126,Player-3725-0BFAE2F6,389684,Player-3725-075D8BFE,6673],35,0,0,0")]
    [InlineData(@"11/28 19:40:57.094  DAMAGE_SPLIT,Player-3725-0669E64A,""Formid - Frostmourne"",0x514,0x0,Player-3725-09FE7744,""Khalous - Frostmourne"",0x40514,0x0,6940,""Blessing of Sacrifice"",0x2,Player-3725-09FE7744,0000000000000000,67569,86120,2586,472,5346,0,0,9741,10000,0,76.88,-900.65,2001,0.0607,246,1302,0,-1,32,0,0,0,nil,nil,nil")]
    public void Test_EventsList(string input)
    {
        var @CombagLogEventComponent = EventGenerator.GetCombatLogEvent<CombatLogEvent>(input);

        switch (@CombagLogEventComponent)
        {
            case SpellCastSuccess spellCastSuccess:
                break;
            case SpellDispel spellDispel:
                spellDispel.Source.UnitName.Should().Be("Naxa - Frostmourne");
                spellDispel.Source.Name.Should().Be("Naxa");
                spellDispel.Source.Server.Should().Be("Frostmourne");
                spellDispel.Spell.Name.Should().Be("Cleanse");
                spellDispel.ExtraSpell.Id.Should().Be(357298);
                break;
            case DragonflightCombatantInfo dragonflightCombatantInfo:
                dragonflightCombatantInfo.ClassTalents.Should().HaveCount(51);
                dragonflightCombatantInfo.EquippedItems.Should().HaveCount(18);
                dragonflightCombatantInfo.InterestingAuras.Should().HaveCount(6);
                Assert.True(dragonflightCombatantInfo.PvPStats is { HonorLevel: 35, Rating: 0, Season: 0, Tier: 0 });
                break;
            case DamageSplit damageSplit:
                damageSplit.Source.Name.Should().Be("Formid");
                damageSplit.Destination.Name.Should().Be("Khalous");
                break;
        }
    }

    [Fact]
    public void Test_FullRaidCombatLogAsync()
    {
        CombatLogParser.Filename = @"TestLogs/Dragonflight/WoWCombatLog.txt";
        var encounters = CombatLogParser.Scan().ToList();
        encounters.Should().NotBeNull().And.HaveCountGreaterThan(1);
        encounters.ForEach(e => OutputEncounterSumary(e));
    }

    [Fact]
    public void Test_ScanMultipleFightsSelectingSecond()
    {
        CombatLogParser.Filename = @"TestLogs/Dragonflight/WoWCombatLog.txt";
        var encounter = CombatLogParser.Scan().Skip(1).Take(1).SingleOrDefault();
        encounter.Should().NotBeNull();
        OutputEncounterSumary(encounter);
    }

    [Fact]
    public void Test_ScanMultipleFights()
    {
        CombatLogParser.Filename = @"TestLogs/Dragonflight/WoWCombatLog.txt";
        var encounters = CombatLogParser.Scan(quickScan: true).ToList();
        encounters.Should().NotBeNull();
        encounters.ForEach(e => OutputEncounterSumary(e));
    }

    [Theory]
    [InlineData(@"11/28 19:36:59.856  ENCOUNTER_END,2431,""Fatescribe Roh - Kalo"",15,14,0,271825", false)]
    [InlineData(@"11/28 19:46:43.635  ENCOUNTER_END,2431,""Fatescribe Roh-Kalo"",15,14,1,404969", true)]
    public void Test_EncounterEnd(string input, bool success)
    {
        var @CombagLogEventComponent = EventGenerator.GetCombatLogEvent<EncounterEnd>(input);
        @CombagLogEventComponent.Success.Should().Be(success);
    }

    [Theory]
    [InlineData(typeof(SpellPeriodicDamage), @"11/28 19:32:36.434  SPELL_PERIODIC_DAMAGE,Creature-0-5047-2450-26923-175730-0000234859,""Fatescribe Roh-Kalo"",0x10a48,0x0,Player-1136-08E79DB6,""Bansky-Gurubashi"",0x512,0x0,353931,""Twist Fate"",0x20,Player-1136-08E79DB6,0000000000000000,50393,54600,2361,327,682,759,17,33,120,0,65.61,-901.65,2001,4.7087,247,4207,7861,-1,32,0,0,1601,nil,nil,nil")]
    [InlineData(typeof(SpellDamage), @"11/28 19:46:43.567  SPELL_DAMAGE,Pet-0-5047-2450-26923-165189-0203600F87,""Gruffhorn"",0x1114,0x0,Creature-0-5047-2450-26923-175730-0000234DDC,""Fatescribe Roh-Kalo"",0x10a48,0x0,83381,""Kill Command"",0x1,Creature-0-5047-2450-26923-175730-0000234DDC,0000000000000000,139,20164970,0,0,1071,0,3,7,100,0,100.86,-931.17,2001,2.9855,63,2245,3021,-1,1,0,0,0,nil,nil,nil")]
    [InlineData(typeof(SpellDamage), @"11/28 19:32:46.738  SPELL_DAMAGE,Player-3725-0BF357DA,""Koriz-Frostmourne"",0x514,0x0,Creature-0-5047-2450-26923-175730-0000234859,""Fatescribe Roh-Kalo"",0x10a48,0x0,285452,""Lava Burst"",0x4,Creature-0-5047-2450-26923-175730-0000234859,0000000000000000,18216931,20164970,0,0,1071,0,3,0,100,0,64.06,-904.28,2001,1.9476,63,8215,3816,-1,4,0,0,0,1,nil,nil")]
    public void Test_DamageSuffix(Type eventType, string input)
    {
        var @CombagLogEventComponent = EventGenerator.GetCombatLogEventAsync<CombatLogEvent>(input).Result;
        @CombagLogEventComponent.GetType().Should().Be(eventType);
    }

    [Theory]
    [InlineData(@"7/31 20:09:49.424  COMBATANT_INFO,Player-3725-0C203472,0,901,9196,26638,2192,0,0,0,1440,1440,1440,250,0,4827,4827,4827,699,4993,2122,2122,2122,5591,263,[(80938,101801,1),(80939,101802,1),(80940,101803,1),(80941,101804,1),(80942,101805,1),(80943,101806,2),(80956,101821,1),(80957,101822,2),(80958,101823,1),(80961,101826,2),(80971,101837,2),(80972,101838,1),(80975,101841,1),(81056,101944,1),(81057,101945,1),(81059,101947,1),(81062,101950,1),(81063,101951,1),(81064,101952,1),(81067,101956,1),(81068,101957,1),(81072,101963,1),(81074,101965,1),(81080,101973,1),(81081,101974,2),(81082,101976,1),(81084,101978,2),(81085,101979,1),(81086,101980,2),(81087,101981,2),(81088,101983,1),(81089,101984,2),(81102,102000,1),(81106,102004,1),(92682,114819,1),(81061,101949,1),(81060,101948,1),(80944,101808,1),(80945,101809,2),(80947,101811,1),(80948,101812,1),(80955,101820,1),(80963,101828,1),(80964,101829,2),(80965,101830,1),(80970,101836,1),(80953,101818,1),(80974,101840,1),(81071,101961,1),(81096,101994,1),(81097,101995,1),(81103,102001,1)],(0,355580,410673,289874),[(202470,441,(),(6652,9414,9229,9409,9334,1498,8767),()),(206180,437,(),(6652,9144,9477,8782,9329,1659,8767),(192958,415,192958,415,192958,415)),(202468,441,(),(6652,9227,9409,9334,1498,8767),()),(0,0,(),(),()),(202473,447,(6625,0,0),(40,9382,9231,1498),()),(193463,447,(),(8836,8840,8902,8960),(192958,415)),(202469,441,(6490,0,0),(6652,9228,9409,9334,1498,8767),()),(193421,447,(6607,0,0),(8836,8840,8902),()),(159356,441,(6574,0,0),(6652,9414,9223,9220,9144,9334,3311,8767),()),(193465,447,(),(8836,8840,8902,8960),()),(192999,447,(6562,0,0),(8836,8840,8902,8780),(192958,415)),(159463,447,(6562,0,0),(9382,6652,9144,3317,8767,9413),(192988,415)),(155881,447,(),(9382,6652,9144,3317,8767),()),(203729,441,(),(9409,6652,9334,1495,8767),()),(133245,441,(6592,0,0),(6652,9223,9220,9144,9334,9458,8767),()),(190513,447,(6643,5401,0),(8836,8840,8902),()),(190518,447,(6643,5400,0),(8836,8840,8902),()),(140579,40,(),(),())],[Player-3725-0C203472,396092,Player-3725-0C203472,411537,Player-3725-0C203472,371172,Player-3725-0C203472,2645,Player-3725-0BE25150,1459,Player-3725-0C1D8956,381756,Player-3725-0C047A40,1126,Player-3725-0BF2915B,6673],51,0,0,0")]
    public void Test_CombatantInfo(string input)
    {
        var @CombagLogEventComponent = EventGenerator.GetCombatLogEvent<DragonflightCombatantInfo>(input);
        output.WriteLine("Interesting Auras on combatant");
        foreach (var aura in @CombagLogEventComponent.InterestingAuras)
        {
            output.WriteLine($"SpellId: {aura.AuraId}");
        }
    }
}

using FluentAssertions;
using System;
using System.Linq;
using System.Threading.Tasks;
using WoWCombatLogParser.Events;
using WoWCombatLogParser.Models;
using Xunit;
using Xunit.Abstractions;
using static WoWCombatLogParser.CombatLogParser;

namespace WoWCombatLogParser.Tests
{
    public class CombatLogParsingTests
    {
        private readonly ITestOutputHelper output;

        public CombatLogParsingTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        private void OutputEncounterSumary(Encounter encounter)
        {
            output.WriteLine($"Event Summary\n{new string('-', 35)}");
            output.WriteLine(encounter.GetEncounterDescription());
            output.WriteLine(new string('=', 35));
            encounter
                .GroupBy(x => x.EventName)
                .OrderBy(x => x.Key)
                .ToList()
                .ForEach(x => output.WriteLine($"{x.Key,-25}{x.Count(),10}"));
            output.WriteLine(new string('-', 35));
            output.WriteLine($"{"Count",-11}{encounter.Count,24}");
            output.WriteLine($"{new string('=', 35)}\n\n");
        }

        [Fact]
        public void Test_RegisteredEventHandlers()
        {
            EventGenerator.GetRegisteredEventHandlers().ForEach(x => output.WriteLine(x));
        }

        [Fact]
        public void Test_SingleEncounter()
        {
            Encounter encounter = ParseCombatLogSegments(@"TestLogs/SingleFightCombatLog.txt").FirstOrDefault();
            encounter.Combatants.Should().HaveCount(14);            
            encounter.GetEncounterDescription().Should().Be("Fatescribe Roh-Kalo Heroic Wipe (4:31)  7:32 PM");

            OutputEncounterSumary(encounter);
        }

        [Fact/*(Skip = "Manual Run Only - Performance test")*/]
        public void Test_MultipleEncounters()
        {
            var encounters = ParseCombatLogSegments(@"TestLogs/WoWCombatLog-112821_193218.txt").ToList();
            encounters.Should().HaveCountGreaterThan(1);
            encounters.ForEach(e => OutputEncounterSumary(e));
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
        [InlineData(@"11/28 19:54:13.422  SPELL_DISPEL,Player-3725-0AF257AE,""Naxa - Frostmourne"",0x514,0x0,Player-3725-06B15901,""Svothgos - Frostmourne"",0x514,0x0,4987,""Cleanse"",0x2,357298,""Frozen Binds"",16,DEBUFF")]
        public void Test_SpellDispel(string input)
        {
            var @event = new CombatLogEvent<Spell, Dispel>(GetConstructorParams(input));
            @event.Parse();
        }

        [Theory]
        [InlineData(@"11/28 19:36:59.856  ENCOUNTER_END,2431,""Fatescribe Roh - Kalo"",15,14,0,271825", false)]
        [InlineData(@"11/28 19:46:43.635  ENCOUNTER_END,2431,""Fatescribe Roh-Kalo"",15,14,1,404969", true)]
        public void Test_EncounterEnd(string input, bool success)
        {
            var @event = new CombatLogEvent<EncounterEnd>(GetConstructorParams(input));
            @event.Parse();
            @event.Event.Success.Should().Be(success);
        }

        [Theory]
        [InlineData(@"11/28 19:32:28.026  COMBATANT_INFO,Player-3725-09C56D56,1,217,1589,2469,496,0,0,0,450,450,450,60,30,865,865,865,39,358,334,334,334,930,263,(117014,201900,260878,210853,196884,197214,262624),(0,193876,204331,204264),[1,3,[],[(844),(849),(850),(858),(863),(864),(995),(1010),(1837),(1839),(1841),(1842)],[(94,239),(111,226),(93,184),(95,239),(110,239),(98,226)]],[(186341,239,(),(7188,6652,1485,6646),(187319,255)),(186291,239,(),(7188,6652,7575,1485,6646),()),(172327,225,(),(6995,6718,6648,6649,1522),()),(0,0,(),(),()),(186303,239,(6230,0,0),(7188,6652,1485,6646),(187320,255)),(186301,239,(),(7188,6652,1485,6646),(187318,255)),(186307,239,(),(7188,6652,1485,6646),()),(186343,239,(6211,0,0),(7188,6652,1485,6646),(187065,255)),(178767,252,(),(7622,7359,6652,7574,1566,6646),()),(186308,239,(),(7188,6652,1485,6646),()),(186377,233,(6166,0,0),(7189,40,7575,1472,6646),()),(186375,239,(6166,0,0),(7188,6652,7575,1485,6646),()),(186432,226,(),(7189,6652,1472,6646),()),(186423,239,(),(7188,6652,1485,6646),()),(186374,239,(6204,0,0),(7188,6652,1485,6646),()),(186388,239,(6228,5401,0),(7188,6652,1485,6646),()),(186387,239,(6229,5400,0),(7188,6652,1485,6646),()),(0,0,(),(),())],[Player-3725-09C56D56,307185,Player-3725-09C56D56,327709,Player-3725-09C56D56,2645,Player-3725-09D57DD8,1459,Player-3725-09C56D56,355794,Player-3725-09D5AE20,21562,Player-3725-09D7A162,6673],23,0,0,0")]
        public async Task Test_CombantInfo(string input)
        {
            var @event = new CombatLogEvent<CombatantInfo>(GetConstructorParams(input));
            await @event.ParseAsync();
            var combatantInfo = @event.Event;
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
            var @event = new DamageSplit(GetConstructorParams(input));
            @event.Parse();           
        }

        [Theory]
        [InlineData(typeof(CombatLogEvent<SpellPeriodic, Damage>), @"11/28 19:32:36.434  SPELL_PERIODIC_DAMAGE,Creature-0-5047-2450-26923-175730-0000234859,""Fatescribe Roh-Kalo"",0x10a48,0x0,Player-1136-08E79DB6,""Bansky-Gurubashi"",0x512,0x0,353931,""Twist Fate"",0x20,Player-1136-08E79DB6,0000000000000000,50393,54600,2361,327,682,759,17,33,120,0,65.61,-901.65,2001,4.7087,247,4207,7861,-1,32,0,0,1601,nil,nil,nil")]
        [InlineData(typeof(CombatLogEvent<Swing, Damage>), @"11/28 19:32:28.108  SWING_DAMAGE,Creature-0-5047-2450-26923-175730-0000234859,""Fatescribe Roh-Kalo"",0xa48,0x0,Player-3725-0BEB73E6,""Reek�-Frostmourne"",0x512,0x0,Creature-0-5047-2450-26923-175730-0000234859,0000000000000000,20164560,20164970,0,0,1071,0,3,0,100,0,69.31,-926.81,2001,1.7816,63,31511,41403,-1,1,0,0,0,nil,nil,nil")]
        [InlineData(typeof(CombatLogEvent<Spell, Damage>), @"11/28 19:32:29.893  SPELL_DAMAGE,Player-3725-09D5AE20,""Esqi -Frostmourne"",0x514,0x0,Creature-0-5047-2450-26923-175730-0000234859,""Fatescribe Roh-Kalo"",0xa48,0x0,8092,""Mind Blast"",0x20,Creature-0-5047-2450-26923-175730-0000234859,0000000000000000,20123688,20164970,0,0,1071,0,3,0,100,0,66.59,-913.53,2001,1.7506,63,3629,3628,-1,32,0,0,0,nil,nil,nil")]
        public void Test_DamageSuffix(Type eventType, string input)
        {
            var @event = EventGenerator.GetCombatLogEvent(GetConstructorParams(input));
            @event.Parse();
            @event.GetType().Should().Be(eventType);
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

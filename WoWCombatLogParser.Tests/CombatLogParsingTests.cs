using System.Linq;
using WoWCombatLogParser.Models;
using Xunit;
using Xunit.Abstractions;
using static WoWCombatLogParser.CombatLogParser;
using FluentAssertions;
using WoWCombatLogParser.Events.Special;
using System.Threading.Tasks;
using WoWCombatLogParser.IO;
using WoWCombatLogParser.Events;

namespace WoWCombatLogParser.Tests
{
    public class CombatLogParsingTests
    {
        private readonly ITestOutputHelper output;

        public CombatLogParsingTests(ITestOutputHelper output)
        {
            this.output = output;
        }
        
        [Theory]
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
        [InlineData(@"[(186341,239,(),(7188,6652,1485,6646),(187319,255)),(186291,239,(),(7188,6652,7575,1485,6646),()),(172327,225,(),(6995,6718,6648,6649,1522),()),(0,0,(),(),()),(186303,239,(6230,0,0),(7188,6652,1485,6646),(187320,255)),(186301,239,(),(7188,6652,1485,6646),(187318,255)),(186307,239,(),(7188,6652,1485,6646),()),(186343,239,(6211,0,0),(7188,6652,1485,6646),(187065,255)),(178767,252,(),(7622,7359,6652,7574,1566,6646),()),(186308,239,(),(7188,6652,1485,6646),()),(186377,233,(6166,0,0),(7189,40,7575,1472,6646),()),(186375,239,(6166,0,0),(7188,6652,7575,1485,6646),()),(186432,226,(),(7189,6652,1472,6646),()),(186423,239,(),(7188,6652,1485,6646),()),(186374,239,(6204,0,0),(7188,6652,1485,6646),()),(186388,239,(6228,5401,0),(7188,6652,1485,6646),()),(186387,239,(6229,5400,0),(7188,6652,1485,6646),()),(0,0,(),(),())]")]
        public void Test_EquippedItems(string input)
        {
            using var tfr = new TextFieldReader(input) { Delimiters = new[] { ',' }, HasFieldsEnclosedInQuotes = true };
            var fields = tfr.ReadFields();

            var equippedItems = new NestedPartList<EquippedItem>();
            using var data = fields.GetEnumerator();
            if (data.MoveNext())
            {
                equippedItems.Parse(data);
            }
            equippedItems.Should().HaveCount(18);
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

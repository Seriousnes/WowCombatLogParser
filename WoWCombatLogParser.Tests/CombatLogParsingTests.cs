using System;
using System.Linq;
using WoWCombatLogParser;
using WoWCombatLogParser.Events.Complex.Prefix;
using WoWCombatLogParser.Events.Complex.Suffix;
using WoWCombatLogParser.Events.Simple;
using WoWCombatLogParser.Models;
using Xunit;

namespace WoWCombatLogParser.Tests
{
    public class CombatLogParsingTests
    {
        [Fact]
        public void TestCombatLogParses()
        {
            var parser = new CombatLogParser();
            parser.Parse(@"TestLogs/WoWCombatLog.txt");

            Assert.True(parser.Events.Count > 0);
        }

        [Fact]
        public void TestFieldOrder()
        {
            var obj = new CombatantInfo();
            var fields = obj.GetType().GetProperties();
            Assert.True(fields.Count() > 0);
        }
    }    
}

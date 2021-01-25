using System;
using System.Linq;
using WoWCombatLogParser;
using WoWCombatLogParser.Events.Complex.Prefix;
using WoWCombatLogParser.Events.Complex.Suffix;
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
            parser.Parse(@"TestLogs/WoWCombatLog_Torghast-20210103-001_Full.txt");

            Assert.True(parser.Events.Count > 0);
        }
    }
}
